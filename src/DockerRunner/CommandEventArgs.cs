using System;

namespace DockerRunner
{
    /// <summary>
    /// Event arguments raised when running a command.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">The name of the command being run.</param>
        /// <param name="arguments">The space separated arguments of the command.</param>
        public CommandEventArgs(string command, string arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        /// <summary>
        /// The name of the command being run.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// The space separated arguments of the command.
        /// </summary>
        public string Arguments { get; }
    }
}