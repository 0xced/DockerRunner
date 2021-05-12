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
        /// <param name="portMappings">A collection of port mapping between the host and the container.</param>
        public ContainerInfo(ContainerId containerId, string host, IReadOnlyCollection<PortMapping> portMappings)
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
        /// <remarks>
        /// It is possible for the list to contain several entries with the same <see cref="PortMapping.HostPort"/> and
        /// <see cref="PortMapping.ContainerPort"/> with only the <see cref="PortMapping.AddressFamily"/> being different.
        /// </remarks>
        /// <example>
        /// // For a container known to expose port 80 such as a web server
        /// <para>
        /// ushort hostPort = containerInfo.PortMappings.First(e => e.ContainerPort == 80).HostPort;
        /// </para>
        /// </example>
        public IReadOnlyCollection<PortMapping> PortMappings { get; }
    }
}