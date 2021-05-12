using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DockerRunner
{
    /// <summary>
    /// Provides docker database container lifecycle management.
    /// </summary>
    public class DockerDatabaseContainerRunner : DockerContainerRunner
    {
        /// <summary>
        /// Use <see cref="StartDockerDatabaseContainerRunnerAsync"/> to create a <see cref="DockerDatabaseContainerRunner"/>.
        /// </summary>
        protected DockerDatabaseContainerRunner(ContainerInfo containerInfo, string connectionString, DockerContainerConfiguration configuration, EventHandler<CommandEventArgs>? runningCommand, EventHandler<RanCommandEventArgs>? ranCommand, bool waitOnDispose) : base(configuration, runningCommand, ranCommand, waitOnDispose)
        {
            ContainerInfo = containerInfo;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Starts a docker database container. This method returns after a successful connection to the database has been established.
        /// </summary>
        /// <param name="configuration">The <see cref="DockerDatabaseContainerConfiguration"/> defining how the container must start.</param>
        /// <param name="runningCommand">An optional event handler raised when a command is running.</param>
        /// <param name="ranCommand">An optional event handler raised when a command has successfully finished running.</param>
        /// <param name="waitOnDispose">
        /// If <c>true</c>, waits for the container to be fully stopped when the runner is disposed, in <see cref="DockerContainerRunner.DisposeAsync"/>.
        /// Using <c>false</c> is faster but no error will be reported if stopping the container fails.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the start operation. Note that the container may actually continue to start.</param>
        /// <returns>The container runner that can be used to stop the container.</returns>
        /// <exception cref="TimeoutException">
        /// If a connection to the database can not be established within the time allowed by the <paramref name="configuration"/> timeout.
        /// The inner exception of the timeout exception contains the database exception that occurred when trying to connect to the database.
        /// </exception>
        public static async Task<DockerDatabaseContainerRunner> StartDockerDatabaseContainerRunnerAsync(
            DockerDatabaseContainerConfiguration configuration,
            EventHandler<CommandEventArgs>? runningCommand = null,
            EventHandler<RanCommandEventArgs>? ranCommand = null,
            bool waitOnDispose = false,
            CancellationToken cancellationToken = default)
        {
            var runner = await StartDockerContainerRunnerAsync(configuration, runningCommand, ranCommand, waitOnDispose, cancellationToken);
            var containerInfo = runner.ContainerInfo;
            var hostEndpoint = GetHostEndpoint(containerInfo.PortMappings, configuration);
            var connectionString = configuration.ConnectionString(hostEndpoint.Address.ToString(), (ushort)hostEndpoint.Port);
            var stopWatch = Stopwatch.StartNew();
            while (true)
            {
                using var connection = configuration.ProviderFactory.CreateConnection() ?? throw new InvalidOperationException($"Failed to create a connection with {configuration.ProviderFactory} because CreateConnection() returned null.");
                connection.ConnectionString = connectionString;
                try
                {
                    await connection.OpenAsync(cancellationToken);
                    return new DockerDatabaseContainerRunner(containerInfo, connectionString, configuration, runningCommand, ranCommand, waitOnDispose);
                }
                catch (Exception exception)
                {
                    await Task.Delay(TimeSpan.FromSeconds(value: 1), cancellationToken);
                    if (stopWatch.Elapsed > configuration.Timeout)
                    {
                        throw new TimeoutException($"Database was not available on \"{connectionString}\" after waiting for {configuration.Timeout.TotalSeconds:F1} seconds.", exception);
                    }
                }
            }
        }

        /// <summary>
        /// The connection string to use to connect to the database.
        /// </summary>
        public string ConnectionString { get; }

        private static IPEndPoint GetHostEndpoint(IReadOnlyCollection<PortMapping> portMappings, DockerDatabaseContainerConfiguration configuration)
        {
            var containerPort = configuration.Port;
            var containerPorts = portMappings.Select(e => e.ContainerPort).Distinct().ToList();
            switch (containerPorts.Count)
            {
                case 0:
                    throw new InvalidOperationException("The docker container does not expose any port.");
                case 1:
                {
                    var portMapping = portMappings.First();
                    if (containerPort.HasValue && containerPort.Value != portMapping.ContainerPort)
                    {
                        throw new InvalidOperationException($"The port defined in the configuration ({containerPort}) does not match the port exposed by the docker container ({portMapping.ContainerPort}). Either change the port to null for automatic detection or to {portMapping.ContainerPort}.");
                    }
                    return portMapping.HostEndpoint;
                }
            }
            var hostEndpoint = portMappings.FirstOrDefault(e => e.ContainerPort == containerPort)?.HostEndpoint;
            if (hostEndpoint == null)
            {
                if (containerPort == null)
                {
                    throw new InvalidOperationException($"The docker container based on image '{configuration.ImageName}' exposes {containerPorts.Count} ports: {string.Join(",", containerPorts)}. Please specify which one to use with the {configuration.GetType().FullName}.{nameof(configuration.Port)} property.");
                }
                throw new InvalidOperationException($"The docker container based on image '{configuration.ImageName}' does not expose port {containerPort}. The exposed ports are {string.Join(",", containerPorts)}. Please specify which one to use with the {configuration.GetType().FullName}.{nameof(configuration.Port)} property.");
            }
            return hostEndpoint;
        }
    }
}