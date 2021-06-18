using System;
using System.Collections.Generic;
using System.Data.Common;

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

        private static readonly ProviderFactoryDescriptor[] SqlServerDbProviderFactoryDescriptors =
        {
            new ProviderFactoryDescriptor("Microsoft.Data.SqlClient.SqlClientFactory", "Microsoft.Data.SqlClient", "Microsoft.Data.SqlClient"),
            new ProviderFactoryDescriptor("System.Data.SqlClient.SqlClientFactory", "System.Data.SqlClient", "System.Data.SqlClient"),
            // Available in the .NET Framework GAC, requires Version + Culture + PublicKeyToken to be explicitly specified
            new ProviderFactoryDescriptor("System.Data.SqlClient.SqlClientFactory", "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", null),
        };

        private readonly Lazy<DbProviderFactory> _providerFactory = new Lazy<DbProviderFactory>(() => DbProviderFactoryReflection.GetProviderFactory(SqlServerDbProviderFactoryDescriptors));

        /// <summary>
        /// The provider factory used for connecting to the database.
        /// </summary>
        /// <remarks>
        /// Searches the <c>SqlClientFactory</c> instance through reflection in order to let the consumer decides
        /// which implementation to use. Supported implementations are from <c>Microsoft.Data.SqlClient</c> and <c>System.Data.SqlClient</c>
        /// packages or the built-in one from <c>System.Data</c> (.NET Framework only).
        /// </remarks>
        /// <exception cref="MissingAssemblyException">Neither <c>Microsoft.Data.SqlClient</c> nor <c>System.Data.SqlClient</c> is referenced.</exception>
        public override DbProviderFactory ProviderFactory => _providerFactory.Value;

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