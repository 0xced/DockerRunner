using System;
using System.Data.Common;

namespace DockerRunner.Database
{
    /// <summary>
    /// The exception thrown if <see cref="DbProviderFactoryReflection.GetProviderFactory"/> fails to load a <see cref="DbProviderFactory"/>.
    /// </summary>
    internal class MissingDbProviderAssemblyException : Exception
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="MissingDbProviderAssemblyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingDbProviderAssemblyException(string message) : base(message)
        {
        }
    }
}