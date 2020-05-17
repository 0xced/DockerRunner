using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DockerRunner
{
    /// <inheritdoc cref="DockerRunner.IDockerContainerRunner"/>
    public class DockerContainerRunner : IDockerContainerRunner, IAsyncDisposable
    {
        private IDockerContainerConfiguration _configuration = null!;
        private EventHandler<CommandEventArgs>? _runningCommand;
        private EventHandler<RanCommandEventArgs>? _ranCommand;
        private bool _waitOnDispose;

        /// <summary>
        /// Get information about the started docker container.
        /// </summary>
        public ContainerInfo ContainerInfo { get; private set; } = null!;

        /// <summary>
        /// Use <see cref="StartDockerContainerRunnerAsync"/> to create a <see cref="DockerContainerRunner"/>.
        /// </summary>
        private DockerContainerRunner()
        {
        }

        /// <summary>
        /// Starts a docker container.
        /// </summary>
        /// <param name="configuration">The <see cref="IDockerContainerConfiguration"/> defining how the container must start.</param>
        /// <param name="runningCommand">An optional event handler raised when a command is running.</param>
        /// <param name="ranCommand">An optional event handler raised when a command has successfully finished running.</param>
        /// <param name="waitOnDispose">
        /// If <c>true</c>, waits for the container to be fully stopped when the runner is disposed, in <see cref="DisposeAsync"/>.
        /// Using <c>false</c> is faster but no error will be reported if stopping the container fails.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the start operation. Note that the container may actually continue to start.</param>
        /// <returns>The container runner that can be used to stop the container.</returns>
        public static async Task<DockerContainerRunner> StartDockerContainerRunnerAsync(
            IDockerContainerConfiguration configuration,
            EventHandler<CommandEventArgs>? runningCommand = null,
            EventHandler<RanCommandEventArgs>? ranCommand = null,
            bool waitOnDispose = false,
            CancellationToken cancellationToken = default)
        {
            var runner = new DockerContainerRunner();
            runner._runningCommand += runningCommand;
            runner._ranCommand += ranCommand;
            runner._waitOnDispose = waitOnDispose;
            runner.ContainerInfo = await runner.StartContainerAsync(configuration, cancellationToken);
            return runner;
        }

        /// <inheritdoc />
        public async Task<ContainerInfo> StartContainerAsync(IDockerContainerConfiguration configuration, CancellationToken cancellationToken = default)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var dockerStartDateTime = DateTime.Now;

            var arguments = GetDockerRunArguments();
            var containerId = (await RunDockerAsync("run " + arguments, cancellationToken: cancellationToken)).output;
            var ports = await DockerContainerGetPortsAsync(dockerStartDateTime, containerId, cancellationToken);

            var hostFormat = Environment.GetEnvironmentVariable("XUNIT_FIXTURE_DOCKER_HOST_FORMAT");
            if (hostFormat != null)
            {
                // The idea is to use {{ .NetworkSettings.Networks.nat.IPAddress }} or whatever network is configured on Docker on CI, e.g. AppVeyor
                // See https://github.com/docker/for-win/issues/204 and https://stackoverflow.com/questions/44817861/windows-container-port-binding-not-working-on-windows-server-2016-using-docker/44827162#44827162
                var address = (await RunDockerAsync($"inspect {containerId} --format={hostFormat}", cancellationToken: cancellationToken)).output;
                return new ContainerInfo(new ContainerId(containerId), address, ports);
            }

            var host = await DockerGetHostAsync(cancellationToken);
            return new ContainerInfo(new ContainerId(containerId), host, ports);
        }

        /// <inheritdoc />
        public async Task StopContainerAsync(ContainerId containerId, bool wait, CancellationToken cancellationToken = default)
        {
            await RunDockerAsync($"stop \"{containerId}\"", waitForExit: wait, cancellationToken: cancellationToken);
        }

        // See https://github.com/docker/compose/blob/1.24.0/compose/config/types.py#L127-L136
        private static string NormalizedPath(FileSystemInfo fileSystemInfo)
        {
            var segments = new Uri(fileSystemInfo.FullName).Segments;
            // The drive must be lowercase, see https://docs.docker.com/toolbox/toolbox_install_windows/#optional-add-shared-directories
            if (segments[1].EndsWith(":/"))
                segments[1] = segments[1].Replace(":", "").ToLower();
            return string.Join("", segments);
        }

        private static string GetMountArguments(DockerStorage storage)
        {
            if (storage is DockerVolume volume)
            {
                return $"--mount type=volume,source=\"{volume.Name}\",destination=\"{volume.Destination}\"";
            }
            if (storage is DockerBindMount bindMount)
            {
                var readOnlySuffix = bindMount.IsReadOnly ? ",readonly" : "";
                return $"--mount type=bind,source=\"{NormalizedPath(bindMount.Source)}\",destination=\"{bindMount.Destination}\"{readOnlySuffix}";
            }
            if (storage is DockerTmpfsMount tmpfsMount)
            {
                return $"--mount type=tmpfs,destination=\"{tmpfsMount.Destination}\"";
            }
            throw new NotSupportedException($"Unsupported {nameof(DockerStorage)} type: {storage.GetType().FullName}");
        }

        private string GetDockerRunArguments()
        {
            var environmentVariablesArguments = _configuration.EnvironmentVariables.Select(e => $"--env \"{e.Key}\"=\"{e.Value}\"");
            var mountArguments = _configuration.Storage.Select(GetMountArguments);
            var arguments = environmentVariablesArguments.Concat(mountArguments)
                .Concat(new []
                {
                    "--publish-all",
                    "--detach",
                    "--rm",
                    $"\"{_configuration.ImageName}\"",
                });
            return string.Join(" ", arguments);
        }

        private async Task<string> DockerGetHostAsync(CancellationToken cancellationToken)
        {
            try
            {
                return (await RunProcessAsync("docker-machine", "ip", cancellationToken: cancellationToken)).output;
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }

        private async Task<IReadOnlyList<PortMapping>> DockerContainerGetPortsAsync(DateTime dockerStartDateTime, string containerId, CancellationToken cancellationToken)
        {
            var portLines = (await RunDockerAsync($"port {containerId}", cancellationToken: cancellationToken)).output;
            using var reader = new StringReader(portLines);
            var ports = new List<PortMapping>();
            string portLine;
            while ((portLine = await reader.ReadLineAsync()) != null)
            {
                var match = Regex.Match(portLine, @"(?<containerPort>\d+)/.* -> 0\.0\.0\.0:(?<hostPort>\d+)");
                var containerPort = match.Groups["containerPort"];
                var hostPort = match.Groups["hostPort"];
                if (!(containerPort.Success && hostPort.Success))
                {
                    string logs;
                    try
                    {
                        var (output, error) = await RunDockerAsync($"logs --since {dockerStartDateTime:O} {containerId}", trimResult: false, cancellationToken: cancellationToken);
                        logs = !string.IsNullOrWhiteSpace(error) ? error : output;
                    }
                    catch
                    {
                        logs = "";
                    }
                    var message = string.IsNullOrWhiteSpace(logs) ? "Please check its logs." : "Here are its logs: " + Environment.NewLine + logs;
                    throw new DockerException($"The container {containerId} failed to start properly. {message}");
                }

                ports.Add(new PortMapping(hostPort: ushort.Parse(hostPort.Value), containerPort: ushort.Parse(containerPort.Value)));
            }
            return ports;
        }

        private async Task<(string output, string error)> RunDockerAsync(string arguments, bool waitForExit = true, bool trimResult = true, CancellationToken cancellationToken = default)
        {
            return await RunProcessAsync("docker", arguments, waitForExit, trimResult, cancellationToken);
        }

        private async Task<(string output, string error)> RunProcessAsync(string command, string arguments, bool waitForExit = true, bool trimResult = true, CancellationToken cancellationToken = default)
        {
            var startInfo = new ProcessStartInfo(command, arguments) { CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
            using var process = new Process {StartInfo = startInfo};
            _runningCommand?.Invoke(this, new CommandEventArgs(command, arguments));
            try
            {
                process.Start();
            }
            catch (Exception exception)
            {
                throw new DockerException($"Failed to run `{command} {arguments}` Is {command} installed?", exception);
            }
            if (waitForExit)
            {
                var exitCode = await process.WaitForExitAsync(cancellationToken);
                var error = await process.StandardError.ReadToEndAsync();
                if (exitCode != 0)
                {
                    throw new DockerCommandException(exitCode, error);
                }
                var output = await process.StandardOutput.ReadToEndAsync();
                _ranCommand?.Invoke(this, new RanCommandEventArgs(command, arguments, output));
                return trimResult ? (output.TrimEnd('\n'), error.TrimEnd('\n')) : (output, error);
            }
            return ("", "");
        }

        /// <summary>
        /// Stops the container.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await StopContainerAsync(ContainerInfo.ContainerId, wait: _waitOnDispose);
        }
    }
}