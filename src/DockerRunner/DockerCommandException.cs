namespace DockerRunner
{
    /// <summary>
    /// Represents errors that occur during execution of the docker command
    /// </summary>
    public class DockerCommandException : DockerException
    {
        /// <inheritdoc />
        public DockerCommandException(CommandEventArgs commandEventArgs, int exitCode, string error) : base(GetMessage(commandEventArgs, exitCode, error))
        {
            CommandEventArgs = commandEventArgs;
            ExitCode = exitCode;
            Error = error;
        }

        private static string GetMessage(CommandEventArgs commandEventArgs, int exitCode, string error)
            => $"Running the command `{commandEventArgs.Command} {commandEventArgs.Arguments}` failed with exit code {exitCode} and produced this error on stderr: {error}";

        /// <summary>
        /// The <see cref="CommandEventArgs"/> that was run.
        /// </summary>
        public CommandEventArgs CommandEventArgs { get; }

        /// <summary>
        /// The exit code of the docker command.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// The error that was written on stderr.
        /// </summary>
        public string Error { get; }
    }
}