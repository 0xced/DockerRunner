using System.Collections.Generic;

namespace DockerRunner
{
    /// <summary>
    /// The ContainerInfo class holds information about a running docker container.
    /// </summary>
    public class ContainerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInfo"/> class.
        /// </summary>
        /// <param name="containerId">The docker container id.</param>
        /// <param name="host">The host that one must connect to in order to reach the docker container.</param>
        /// <param name="portMappings">A list of port mapping between the host and the container.</param>
        public ContainerInfo(ContainerId containerId, string host, IReadOnlyList<PortMapping> portMappings)
        {
            ContainerId = containerId;
            Host = host;
            PortMappings = portMappings;
        }

        /// <summary>
        /// The docker container id.
        /// </summary>
        public ContainerId ContainerId { get; }

        /// <summary>
        /// The host that one must connect to in order to reach the docker container.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// A list of <see cref="PortMapping"/> between the host and the container.
        /// Use the port mapping to determine which port you should connect to on the host.
        /// </summary>
        /// <example>
        /// // For a container known to expose port 80 such as a web server
        /// var hostPort = containerInfo.PortMappings.Single(e => e.ContainerPort == 80).HostPort;
        /// </example>
        public IReadOnlyList<PortMapping> PortMappings { get; }
    }
}