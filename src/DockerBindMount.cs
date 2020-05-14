using System.IO;

namespace DockerRunner
{
    /// <summary>
    /// Docker bind mount for sharing data between the container and the host machine file system.
    /// </summary>
    /// <remarks>See [Use bind mounts](https://docs.docker.com/storage/bind-mounts/) for mor information.</remarks>
    public sealed class DockerBindMount : DockerStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockerBindMount"/> class.
        /// </summary>
        /// <param name="source">The path to the file or directory on the host machine.</param>
        /// <param name="destination">The path where the file or directory is mounted in the container.</param>
        /// <param name="isReadOnly">Causes the bind mount to be mounted into the container as read-only when <c>true</c>.</param>
        public DockerBindMount(FileSystemInfo source, string destination, bool isReadOnly)
        {
            Source = source;
            Destination = destination;
            IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// The path to the file or directory on the host machine.
        /// </summary>
        public FileSystemInfo Source { get; }

        /// <summary>
        /// Causes the bind mount to be mounted into the container as read-only when <c>true</c>.
        /// </summary>
        public bool IsReadOnly { get; }
    }
}