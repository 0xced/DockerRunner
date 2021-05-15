using System;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace DockerRunner.Tests
{
    public class MariaDBConfiguration : DockerDatabaseContainerConfiguration
    {
        protected virtual string Database => "database";
        protected virtual string User => "root";
        protected virtual string Password => "docker";

        /// <inheritdoc />
        public override string ConnectionString(string host, ushort port)
        {
            var builder = new MySqlConnectionStringBuilder
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
        public override DbProviderFactory ProviderFactory => MySqlClientFactory.Instance;

        /// <inheritdoc />
        public override string ImageName => "mariadb";

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["MARIADB_DATABASE"] = Database,
            ["MARIADB_ROOT_PASSWORD"] = Password,
        };

        public override TimeSpan Timeout => TimeSpan.FromMinutes(1);
    }
}