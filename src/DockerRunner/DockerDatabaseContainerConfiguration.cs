using System;
using System.Data.Common;

namespace DockerRunner
{
    /// <summary>
    /// Provides the required information to connect to a docker database.
    /// </summary>
    public abstract class DockerDatabaseContainerConfiguration : DockerContainerConfiguration
    {
        /// <param name="host">The host on which the database is running.</param>
        /// <param name="port">The port on which the database is listening for connections.</param>
        /// <returns>The connection string for the given <paramref name="host"/> and <paramref name="port"/>.</returns>
        public abstract string ConnectionString(string host, ushort port);

        /// <summary>
        /// The provider factory used for connecting to the database.
        /// </summary>
        public abstract DbProviderFactory ProviderFactory { get; }

        /// <summary>
        /// The exposed database port by the docker container. See the documentation of the docker image for exposed ports.
        /// </summary>
        /// <remarks>May be null if and only if the docker container exposes a single port.</remarks>
        /// <example>3306</example>
        public virtual ushort? Port { get; } = null;

        /// <summary>
        /// The amount of time to wait for the database to be available after the docker container has started before giving up.
        /// </summary>
        public virtual TimeSpan Timeout { get; } = TimeSpan.FromSeconds(15);
    }
}