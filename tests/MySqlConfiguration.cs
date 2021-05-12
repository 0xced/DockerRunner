using System;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace DockerRunner.Tests
{
    public class MySqlConfiguration : DockerDatabaseContainerConfiguration
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
        public override ushort? Port => 3306;

        /// <inheritdoc />
        public override string ImageName => "mysql/mysql-server";

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["MYSQL_DATABASE"] = Database,
            ["MYSQL_ROOT_PASSWORD"] = Password,
            ["MYSQL_ROOT_HOST"] = "%",
        };

        public override TimeSpan Timeout => TimeSpan.FromMinutes(1);
    }
}