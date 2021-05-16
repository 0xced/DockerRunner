using System.Collections.Generic;
using System.Data.Common;
using Npgsql;

namespace DockerRunner.Database.Npgsql
{
    /// <summary>
    /// Base configuration for the [PostgreSQL docker image](https://hub.docker.com/_/postgres)
    /// defining everything required for Postgres to run except for <see cref="DockerContainerConfiguration.ImageName"/>
    /// which must be defined in subclasses.
    /// </summary>
    public abstract class PostgresConfigurationBase : DockerDatabaseContainerConfiguration
    {
        private const string Database = "database";
        private const string User = "postgres";
        private const string Password = "docker";

        /// <inheritdoc />
        public override string ConnectionString(string host, ushort port)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = Database,
                Username = User,
                Password = Password,
            };
            return builder.ConnectionString;
        }

        /// <inheritdoc />
        public override DbProviderFactory ProviderFactory => NpgsqlFactory.Instance;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["POSTGRES_DB"] = Database,
            ["POSTGRES_USER"] = User,
            ["POSTGRES_PASSWORD"] = Password,
        };
    }
}