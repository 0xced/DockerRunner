using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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

        private static readonly IEnumerable<string> MySqlClientFactoryTypeNames = new[]
        {
            // From https://www.nuget.org/packages/MySql.Data/
            "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data",
            // From https://www.nuget.org/packages/MySqlConnector/ < 1.0.0
            "MySql.Data.MySqlClient.MySqlClientFactory, MySqlConnector",
            // From https://www.nuget.org/packages/MySqlConnector/ >= 1.0.0
            "MySqlConnector.MySqlConnectorFactory, MySqlConnector",
        };

        /// <summary>
        /// The provider factory used for connecting to the database.
        /// </summary>
        /// <remarks>
        /// Searches the <c>MySqlClientFactory</c> or <c>MySqlConnectorFactory</c> instance through reflection in order to let the consumer decides
        /// which implementation to use. Supported implementations are from <c>MySql.Data</c> and <c>MySqlConnector</c> packages.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Neither <c>MySql.Data</c> nor <c>MySqlConnector</c> is referenced.</exception>
        public override DbProviderFactory ProviderFactory
        {
            get
            {
                foreach (var mySqlClientFactoryTypeName in MySqlClientFactoryTypeNames)
                {
                    var mySqlClientFactoryType = Type.GetType(mySqlClientFactoryTypeName, throwOnError: false);
                    var instance = mySqlClientFactoryType?.GetField("Instance")?.GetValue(null);
                    if (instance != null)
                    {
                        return (DbProviderFactory)instance;
                    }
                }

                var message = $@"Make sure to add a package reference to either ""MySql.Data"" or ""MySqlConnector"" in your project.
The following types were tried to get the `MySqlClientFactory.Instance` through reflection but none were found:
{string.Join(Environment.NewLine, MySqlClientFactoryTypeNames.Select(e => $"  * {e}"))}";
                throw new InvalidOperationException(message);
            }
        }

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