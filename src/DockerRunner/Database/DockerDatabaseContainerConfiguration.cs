using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DockerRunner.Database
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
        /// A collection of SQL statements to execute upon successfully connecting to the database.
        /// <para>
        /// May be used to populate data in docker containers that don't support database creation on startup such
        /// as the official SQL Server docker image. See https://github.com/microsoft/mssql-docker/issues/2
        /// </para>
        /// </summary>
        public virtual IEnumerable<string> SqlStatements { get; } = Enumerable.Empty<string>();

        /// <summary>
        /// The exposed database port by the docker container. See the documentation of the docker image for exposed ports.
        /// </summary>
        /// <remarks>May be null if and only if the docker container exposes a single port.</remarks>
        /// <example>3306</example>
        public virtual ushort? Port { get; } = null;

        /// <summary>
        /// The amount of time to wait for the database to be available after the docker container has started before giving up.
        /// <para>Defaults to 30 seconds.</para>
        /// </summary>
        public virtual TimeSpan Timeout { get; } = TimeSpan.FromSeconds(30);
    }
}