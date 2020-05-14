namespace DockerRunner
{
    /// <summary>
    /// Event arguments raised when a command has successfully finished running.
    /// </summary>
    public class RanCommandEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="RanCommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">The name of the command being run.</param>
        /// <param name="arguments">The space separated arguments of the command.</param>
        /// <param name="output">The captured standard output of the command.</param>
        public RanCommandEventArgs(string command, string arguments, string output) : base(command, arguments)
        {
            Output = output;
        }

        /// <summary>
        /// The captured standard output of the command.
        /// </summary>
        public string Output { get; }
    }
}