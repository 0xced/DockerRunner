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
        /// <param name="hostPort">The port on the host machine.</param>
        /// <param name="containerPort">The port exposed by the docker container.</param>
        public PortMapping(ushort hostPort, ushort containerPort)
        {
            HostPort = hostPort;
            ContainerPort = containerPort;
        }

        /// <summary>
        /// The port on the host machine.
        /// </summary>
        public ushort HostPort { get; }

        /// <summary>
        /// The port exposed by the docker container.
        /// </summary>
        public ushort ContainerPort { get; }
    }
}