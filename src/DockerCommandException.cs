namespace DockerRunner
{
    /// <summary>
    /// Represents errors that occur during execution of the docker command
    /// </summary>
    public class DockerCommandException : DockerException
    {
        /// <inheritdoc />
        public DockerCommandException(int exitCode, string message) : base(message)
        {
            ExitCode = exitCode;
        }

        /// <summary>
        /// The exit code of the docker command.
        /// </summary>
        public int ExitCode { get; }
    }
}