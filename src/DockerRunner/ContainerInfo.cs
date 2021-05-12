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
        /// <param name="portMappings">A collection of port mapping between the host and the container.</param>
        public ContainerInfo(ContainerId containerId, IReadOnlyCollection<PortMapping> portMappings)
        {
            ContainerId = containerId;
            PortMappings = portMappings;
        }

        /// <summary>
        /// The docker container id.
        /// </summary>
        public ContainerId ContainerId { get; }

        /// <summary>
        /// A list of <see cref="PortMapping"/> between the host and the container.
        /// Use the port mapping to determine which endpoint you should use to reach the container.
        /// </summary>
        /// <remarks>
        /// It is possible for the list to contain several entries with the same <see cref="PortMapping.ContainerPort"/>.
        /// </remarks>
        /// <example>
        /// // For a container known to expose port 80 such as a web server
        /// <para>
        /// IPEndPoint hostEndpoint = containerInfo.PortMappings.First(e => e.ContainerPort == 80).HostEndpoint;
        /// </para>
        /// </example>
        public IReadOnlyCollection<PortMapping> PortMappings { get; }

        /// <inheritdoc />
        public override string ToString() => ContainerId.ToString();
    }
}