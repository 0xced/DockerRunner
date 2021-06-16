using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DockerRunner.Database.MySql
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
            => $"server={host};port={port};database={Database};user id={User};password={Password}";

        private static readonly ProviderFactoryDescriptor[] MySqlDbProviderFactoryDescriptors =
        {
            new ProviderFactoryDescriptor("MySql.Data.MySqlClient.MySqlClientFactory", "MySql.Data", "MySql.Data"),
            new ProviderFactoryDescriptor("MySqlConnector.MySqlConnectorFactory", "MySqlConnector", "MySqlConnector"), // MySqlConnector >= 1.0.0
            new ProviderFactoryDescriptor("MySql.Data.MySqlClient.MySqlClientFactory", "MySqlConnector", "MySqlConnector"), // MySqlConnector < 1.0.0
        };

        private readonly Lazy<DbProviderFactory> _providerFactory = new Lazy<DbProviderFactory>(() => DbProviderFactoryReflection.GetProviderFactory(MySqlDbProviderFactoryDescriptors));

        /// <summary>
        /// The provider factory used for connecting to the database.
        /// </summary>
        /// <remarks>
        /// Searches the <c>MySqlClientFactory</c> or <c>MySqlConnectorFactory</c> instance through reflection in order to let the consumer decides
        /// which implementation to use. Supported implementations are from <c>MySql.Data</c> and <c>MySqlConnector</c> packages.
        /// </remarks>
        /// <exception cref="MissingAssemblyException">Neither <c>MySql.Data</c> nor <c>MySqlConnector</c> is referenced.</exception>
        public override DbProviderFactory ProviderFactory => _providerFactory.Value;

        /// <inheritdoc />
        public override ushort? Port => 3306;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>
        {
            ["MYSQL_DATABASE"] = Database,
            ["MYSQL_ROOT_PASSWORD"] = Password,
            ["MYSQL_ROOT_HOST"] = "%",
        };
    }
}