namespace DockerRunner
{
    /// <summary>
    /// A docker storage to mount when starting the docker container.
    /// Can be either a <see cref="DockerVolume"/>, a <see cref="DockerBindMount"/> or a <see cref="DockerTmpfsMount"/>.
    /// </summary>
    /// <remarks>See [Manage data in Docker](https://docs.docker.com/storage/) for mor information.</remarks>
    public abstract class DockerStorage
    {
        /// <summary>
        /// The path where the file or directory is mounted in the container.
        /// </summary>
        public virtual string Destination { get; protected set; } = null!;
    }
}