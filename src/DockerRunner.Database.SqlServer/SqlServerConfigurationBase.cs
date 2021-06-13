using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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
        public override string ConnectionString(string host, ushort port) => $"Data Source={host},{port};User ID={User};Password={Password}";

        private static readonly IEnumerable<string> SqlClientFactoryTypeNames = new[]
        {
            // From https://www.nuget.org/packages/Microsoft.Data.SqlClient/
            "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient",
            // From https://www.nuget.org/packages/System.Data.SqlClient/
            "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
            // From the .NET Framework GAC
            "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
        };

        /// <summary>
        /// The provider factory used for connecting to the database.
        /// </summary>
        /// <remarks>
        /// Searches the <c>SqlClientFactory</c> instance through reflection in order to let the consumer decides
        /// which implementation to use. Supported implementations are from <c>Microsoft.Data.SqlClient</c> and <c>System.Data.SqlClient</c>
        /// packages or the built-in one from <c>System.Data</c> (.NET Framework only).
        /// </remarks>
        /// <exception cref="InvalidOperationException">Neither <c>Microsoft.Data.SqlClient</c> nor <c>System.Data.SqlClient</c> is referenced.</exception>
        public override DbProviderFactory ProviderFactory
        {
            get
            {
                foreach (var sqlClientFactoryTypeName in SqlClientFactoryTypeNames)
                {
                    var sqlClientFactoryType = Type.GetType(sqlClientFactoryTypeName, throwOnError: false);
                    var instance = sqlClientFactoryType?.GetField("Instance")?.GetValue(null);
                    if (instance != null)
                    {
                        return (DbProviderFactory)instance;
                    }
                }

                var message = $@"Make sure to add a package reference to either ""Microsoft.Data.SqlClient"" or ""System.Data.SqlClient"" in your project.
The following types were tried to get the `SqlClientFactory.Instance` through reflection but none were found:
{string.Join(Environment.NewLine, SqlClientFactoryTypeNames.Select(e => $"  * {e}"))}";
                throw new InvalidOperationException(message);
            }
        }

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