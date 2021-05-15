using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DockerRunner
{
    /// <summary>
    /// Provides docker container lifecycle management.
    /// </summary>
    public class DockerContainerRunner : IAsyncDisposable
    {
        private readonly DockerContainerConfiguration _configuration;
        private readonly EventHandler<CommandEventArgs>? _runningCommand;
        private readonly EventHandler<RanCommandEventArgs>? _ranCommand;
        private readonly bool _waitOnDispose;

        /// <summary>
        /// Get information about the started docker container.
        /// </summary>
        public ContainerInfo ContainerInfo { get; private set; } = null!;

        /// <summary>
        /// Use <see cref="StartAsync"/> to create a <see cref="DockerContainerRunner"/>.
        /// </summary>
        private DockerContainerRunner(
            DockerContainerConfiguration configuration,
            EventHandler<CommandEventArgs>? runningCommand,
            EventHandler<RanCommandEventArgs>? ranCommand,
            bool waitOnDispose)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _runningCommand = runningCommand;
            _ranCommand = ranCommand;
            _waitOnDispose = waitOnDispose;
        }

        /// <summary>
        /// Starts a docker container.
        /// </summary>
        /// <param name="configuration">The <see cref="DockerContainerConfiguration"/> defining how the container must start.</param>
        /// <param name="runningCommand">An optional event handler raised when a command is running.</param>
        /// <param name="ranCommand">An optional event handler raised when a command has successfully finished running.</param>
        /// <param name="waitOnDispose">
        /// If <c>true</c>, waits for the container to be fully stopped when the runner is disposed, in <see cref="DisposeAsync"/>.
        /// Using <c>false</c> is faster but no error will be reported if stopping the container fails.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the start operation. Note that the container may actually continue to start.</param>
        /// <returns>The container runner that can be used to stop the container.</returns>
        public static async Task<DockerContainerRunner> StartAsync(
            DockerContainerConfiguration configuration,
            EventHandler<CommandEventArgs>? runningCommand = null,
            EventHandler<RanCommandEventArgs>? ranCommand = null,
            bool waitOnDispose = false,
            CancellationToken cancellationToken = default)
        {
            var runner = new DockerContainerRunner(configuration, runningCommand, ranCommand, waitOnDispose);
            runner.ContainerInfo = await runner.StartContainerAsync(cancellationToken);
            return runner;
        }

        /// <summary>
        /// Starts a docker container.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the start operation. Note that the container may actually continue to start.</param>
        /// <returns>The <see cref="ContainerInfo"/> holding information about the started docker container.</returns>
        private async Task<ContainerInfo> StartContainerAsync(CancellationToken cancellationToken = default)
        {
            await RunDockerAsync($"pull {QuoteIfNeeded(_configuration.ImageName)}", cancellationToken: cancellationToken);

            var dockerStartDateTime = DateTime.Now;
            var arguments = GetDockerRunArguments();
            var containerId = (await RunDockerAsync("run " + arguments, cancellationToken: cancellationToken)).output;
            var ports = await DockerContainerGetPortsAsync(dockerStartDateTime, containerId, cancellationToken);

            return new ContainerInfo(new ContainerId(containerId), ports);
        }

        /// <summary>
        /// Stops the container.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await RunDockerAsync($"stop {QuoteIfNeeded(ContainerInfo.ContainerId.ToString())}", waitForExit: _waitOnDispose);
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
                return $"--mount type=volume,source={QuoteIfNeeded(volume.Name)},destination={QuoteIfNeeded(volume.Destination)}";
            }
            if (storage is DockerBindMount bindMount)
            {
                var readOnlySuffix = bindMount.IsReadOnly ? ",readonly" : "";
                return $"--mount type=bind,source={QuoteIfNeeded(NormalizedPath(bindMount.Source))},destination={QuoteIfNeeded(bindMount.Destination)}{readOnlySuffix}";
            }
            if (storage is DockerTmpfsMount tmpfsMount)
            {
                return $"--mount type=tmpfs,destination={QuoteIfNeeded(tmpfsMount.Destination)}";
            }
            throw new NotSupportedException($"Unsupported {nameof(DockerStorage)} type: {storage.GetType().FullName}");
        }

        private string GetDockerRunArguments()
        {
            var environmentVariablesArguments = _configuration.EnvironmentVariables.Select(e => $"--env {QuoteIfNeeded(e.Key)}={QuoteIfNeeded(e.Value)}");
            var mountArguments = _configuration.Storage.Select(GetMountArguments);
            var exposeArguments = _configuration.ExposePorts.Select(e => $"--expose {e}");
            var containerName = _configuration.ContainerName;
            var nameArguments = string.IsNullOrEmpty(containerName) ? Enumerable.Empty<string>() : new[] { $"--name {QuoteIfNeeded(containerName!)}" };
            var arguments = environmentVariablesArguments
                .Concat(mountArguments)
                .Concat(exposeArguments)
                .Concat(nameArguments)
                .Concat(new[]
                {
                    "--publish-all",
                    "--detach",
                    "--rm",
                    $"{QuoteIfNeeded(_configuration.ImageName)}",
                });
            return string.Join(" ", arguments);
        }

        private async Task<IReadOnlyList<PortMapping>> DockerContainerGetPortsAsync(DateTime dockerStartDateTime, string containerId, CancellationToken cancellationToken)
        {
            var portLines = (await RunDockerAsync($"port {QuoteIfNeeded(containerId)}", cancellationToken: cancellationToken)).output;
            using var reader = new StringReader(portLines);
            var ports = new List<PortMapping>();
            string portLine;
            while ((portLine = await reader.ReadLineAsync()) != null)
            {
                var match = Regex.Match(portLine, @"(?<containerPort>\d+)/.* -> (?<ipAddress>.*):(?<hostPort>\d+)");
                var containerPort = match.Groups["containerPort"];
                var ipAddress = match.Groups["ipAddress"];
                var hostPort = match.Groups["hostPort"];
                if (!match.Success)
                {
                    string logs;
                    try
                    {
                        var (output, error) = await RunDockerAsync($"logs --since {dockerStartDateTime:O} {QuoteIfNeeded(containerId)}", trimResult: false, cancellationToken: cancellationToken);
                        logs = !string.IsNullOrWhiteSpace(error) ? error : output;
                    }
                    catch
                    {
                        logs = "";
                    }
                    var message = string.IsNullOrWhiteSpace(logs) ? "Please check its logs." : "Here are its logs: " + Environment.NewLine + logs;
                    throw new DockerException($"Failed to retrieve the port mapping for container {containerId}. Maybe the container failed to start properly? {message}");
                }

                if (!IPAddress.TryParse(ipAddress.Value, out var address))
                {
                    throw new DockerException($"Failed to retrieve the port mapping for container {containerId} because '{ipAddress.Value}' could not be parsed as an IPAddress.");
                }

                var endpointAddress = address.AddressFamily switch
                {
                    AddressFamily.InterNetwork => IPAddress.Loopback,
                    AddressFamily.InterNetworkV6 => IPAddress.IPv6Loopback,
                    _ => throw new DockerException($"Failed to retrieve the port mapping for container {containerId} because the IP address family ({address.AddressFamily}) of '{ipAddress.Value}' is not supported.")
                };
                var hostEndpoint = new IPEndPoint(endpointAddress, ushort.Parse(hostPort.Value, NumberStyles.None, CultureInfo.InvariantCulture));

                ports.Add(new PortMapping(hostEndpoint, containerPort: ushort.Parse(containerPort.Value, NumberStyles.None, CultureInfo.InvariantCulture)));
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
            var commandEventArgs = new CommandEventArgs(command, arguments);
            _runningCommand?.Invoke(this, commandEventArgs);
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
                    throw new DockerCommandException(commandEventArgs, exitCode, error);
                }
                var output = await process.StandardOutput.ReadToEndAsync();
                _ranCommand?.Invoke(this, new RanCommandEventArgs(command, arguments, output));
                return trimResult ? (output.TrimEnd('\n'), error.TrimEnd('\n')) : (output, error);
            }
            return ("", "");
        }

        private static string QuoteIfNeeded(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.Contains(" "))
            {
                return "\"" + text + "\"";
            }

            return text;
        }
    }
}