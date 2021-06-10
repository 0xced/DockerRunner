using System;
using System.Collections.Generic;
using System.Data.Common;

#if DRIVER_MYSQLDATA
namespace DockerRunner.Database.MySql
#elif DRIVER_MYSQLCONNECTOR
namespace DockerRunner.Database.MySqlConnector
#else
#error Either DRIVER_MYSQLDATA or DRIVER_MYSQLCONNECTOR must be defined
#endif
{
    /// <summary>
    /// Base configuration for MySQL and MariaDB docker images defining everything required for
    /// MySQL/MariaDB to run except for <see cref="DockerContainerConfiguration.ImageName"/>
    /// which must be defined in subclasses.
    /// </summary>
    public abstract class MySqlConfigurationBase : DockerDatabaseContainerConfiguration
    {
        private const string Database = "database";
        private const string User = "root";
        private const string Password = "docker";

        /// <inheritdoc />
        public override string ConnectionString(string host, ushort port)
        {
#if DRIVER_MYSQLDATA
            var builder = new global::MySql.Data.MySqlClient.MySqlConnectionStringBuilder
#elif DRIVER_MYSQLCONNECTOR
            var builder = new global::MySqlConnector.MySqlConnectionStringBuilder
#endif
            {
                Server = host,
                Port = port,
                Database = Database,
                UserID = User,
                Password = Password,
            };
            return builder.ConnectionString;
        }

        /// <inheritdoc />
#if DRIVER_MYSQLDATA
        public override DbProviderFactory ProviderFactory => global::MySql.Data.MySqlClient.MySqlClientFactory.Instance;
#elif DRIVER_MYSQLCONNECTOR
        public override DbProviderFactory ProviderFactory => global::MySqlConnector.MySqlConnectorFactory.Instance;
#endif

        /// <inheritdoc />
        public override ushort? Port => 3306;

        /// <inheritdoc />
        public override TimeSpan Timeout => TimeSpan.FromMinutes(1);

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["MYSQL_DATABASE"] = Database,
            ["MYSQL_ROOT_PASSWORD"] = Password,
            ["MYSQL_ROOT_HOST"] = "%",
        };
    }
}