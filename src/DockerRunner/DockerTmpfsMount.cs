namespace DockerRunner
{
    /// <summary>
    /// Docker tmpfs mount for persisting data in the host memory.
    /// When the container stops, the tmpfs mount is removed, and files written there won’t be persisted.
    /// </summary>
    /// <remarks>See [Use tmpfs mounts](https://docs.docker.com/storage/tmpfs/) for mor information.</remarks>
    public sealed class DockerTmpfsMount : DockerStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockerTmpfsMount"/> class.
        /// </summary>
        /// <param name="destination">The path where the tmpfs mount is mounted in the container.</param>
        public DockerTmpfsMount(string destination)
        {
            Destination = destination;
        }

        /// <summary>
        /// The path where the tmpfs mount is mounted in the container.
        /// </summary>
        public override string Destination { get; protected set; }
    }
}