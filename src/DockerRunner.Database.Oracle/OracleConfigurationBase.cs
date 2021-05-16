using System;
using System.Collections.Generic;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace DockerRunner.Database.Oracle
{
    /// <summary>
    /// Base configuration for the [Oracle docker image](https://hub.docker.com/r/gvenzl/oracle-xe)
    /// defining everything required for Oracle to run except for <see cref="DockerContainerConfiguration.ImageName"/>
    /// which must be defined in subclasses.
    /// </summary>
    public abstract class OracleConfigurationBase : DockerDatabaseContainerConfiguration
    {
        private const string User = "oracle";
        private const string Password = "docker";

        /// <inheritdoc />
        public override string ConnectionString(string host, ushort port)
        {
            var builder = new OracleConnectionStringBuilder
            {
                DataSource = $"(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={ServiceName})))",
                UserID = User,
                Password = Password,
            };
            return builder.ConnectionString;
        }

        /// <summary>
        /// The Oracle service name.
        /// </summary>
        public abstract string ServiceName { get; }

        /// <inheritdoc />
        public override DbProviderFactory ProviderFactory => OracleClientFactory.Instance;

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