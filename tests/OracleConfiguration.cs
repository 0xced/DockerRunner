using System;
using System.Collections.Generic;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace DockerRunner.Tests
{
    public class OracleConfiguration : DockerDatabaseContainerConfiguration
    {
        protected virtual string User => "oracle";
        protected virtual string Password => "docker";

        /// <inheritdoc />
        public override string ConnectionString(string host, ushort port)
        {
            var builder = new OracleConnectionStringBuilder
            {
                DataSource = $"(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={host})(PORT={port})))",
                UserID = User,
                Password = Password,
            };
            return builder.ConnectionString;
        }

        /// <inheritdoc />
        public override DbProviderFactory ProviderFactory => OracleClientFactory.Instance;

        /// <summary>
        /// The official Oracle image from https://hub.docker.com/r/gvenzl/oracle-xe
        /// </summary>
        public override string ImageName => "gvenzl/oracle-xe:11-slim";

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["APP_USER"] = User,
            ["APP_USER_PASSWORD"] = Password,
            ["ORACLE_RANDOM_PASSWORD"] = "true",
        };

        /// <inheritdoc />
        public override IEnumerable<ushort> ExposePorts => new ushort[] { 1521 };

        /// <inheritdoc />
        public override TimeSpan Timeout => TimeSpan.FromMinutes(1);
    }
}