using System;
using System.Collections.Generic;
using System.Data.Common;
using Npgsql;

namespace DockerRunner.Tests
{
    public class PostgresConfiguration : DockerDatabaseContainerConfiguration
    {
        protected virtual Version? PostgresVersion { get; }
        protected virtual bool Alpine { get; }

        protected virtual string Database => "database";
        protected virtual string User => "postgres";
        protected virtual string Password => "docker";

        public PostgresConfiguration() : this(postgresVersion: null, alpine: false)
        {
        }

        public PostgresConfiguration(Version? postgresVersion, bool alpine)
        {
            PostgresVersion = postgresVersion;
            Alpine = alpine;
        }

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
        public override string ImageName
        {
            get
            {
                if (PostgresVersion == null)
                {
                    return Alpine ? "postgres:alpine" : "postgres";
                }
                var version = PostgresVersion.Minor == 0 ? PostgresVersion.Major.ToString() : PostgresVersion.ToString();
                return $"postgres:{version}" + (Alpine ? "-alpine" : "");
            }
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["POSTGRES_DB"] = Database,
            ["POSTGRES_USER"] = User,
            ["POSTGRES_PASSWORD"] = Password,
        };
    }
}