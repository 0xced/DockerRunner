using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DockerRunner.Database.SqlServer
{
    /// <summary>
    /// Base configuration for the [Microsoft SQL Server docker image](https://hub.docker.com/_/microsoft-mssql-server)
    /// defining everything required for SQL Server to run except for <see cref="DockerContainerConfiguration.ImageName"/>
    /// which must be defined in subclasses.
    /// </summary>
    public abstract class SqlServerConfigurationBase : DockerDatabaseContainerConfiguration
    {
        private const string User = "sa";
        // SQL Server password policy requirements: the password must be at least 8 characters long and contain characters from three of the following four sets: Uppercase letters, Lowercase letters, Base 10 digits, and Symbols.
        private const string Password = "Docker(!)";

        /// <inheritdoc />
        public override string ConnectionString(string host, ushort port)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = $"{host},{port}",
                UserID = User,
                Password = Password,
            };
            return builder.ConnectionString;
        }

        /// <inheritdoc />
        public override DbProviderFactory ProviderFactory => SqlClientFactory.Instance;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["ACCEPT_EULA"] = "Y",
            ["SA_PASSWORD"] = Password,
        };

        /// <inheritdoc />
        public override TimeSpan Timeout => TimeSpan.FromMinutes(1);
    }
}