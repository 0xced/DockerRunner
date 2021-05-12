using System.Net;

namespace DockerRunner
{
    /// <summary>
    /// Port mapping between the host machine and the docker container.
    /// </summary>
    public class PortMapping
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="PortMapping"/> class.
        /// </summary>
        /// <param name="hostEndpoint">The <see cref="IPEndPoint"/> to use in order to reach the docker container on <paramref name="containerPort"/>.</param>
        /// <param name="containerPort">The port exposed by the docker container.</param>
        public PortMapping(IPEndPoint hostEndpoint, ushort containerPort)
        {
            HostEndpoint = hostEndpoint;
            ContainerPort = containerPort;
        }

        /// <summary>
        /// The port exposed by the docker container.
        /// </summary>
        public ushort ContainerPort { get; }

        /// <summary>
        /// The <see cref="IPEndPoint"/> to use in order to reach the docker container on <see cref="ContainerPort"/>.
        /// </summary>
        public IPEndPoint HostEndpoint { get; }
    }
}