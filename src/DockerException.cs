using System;

namespace DockerRunner
{
    /// <summary>
    /// Represents errors that occur during docker execution
    /// </summary>
    public class DockerException : Exception
    {
        /// <inheritdoc />
        public DockerException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public DockerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}