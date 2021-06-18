using System;
using System.Collections.Generic;
using System.Data.Common;

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
            => $"USER ID={User};PASSWORD={Password};DATA SOURCE=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={ServiceName})))";

        /// <summary>
        /// The Oracle service name.
        /// </summary>
        public abstract string ServiceName { get; }

        private static readonly DbProviderFactoryDescriptor[] OracleDbProviderFactoryDescriptors =
        {
            new DbProviderFactoryDescriptor("Oracle.ManagedDataAccess.Client.OracleClientFactory", "Oracle.ManagedDataAccess", "Oracle.ManagedDataAccess.Core"),
            new DbProviderFactoryDescriptor("Oracle.ManagedDataAccess.Client.OracleClientFactory", "Oracle.ManagedDataAccess", "Oracle.ManagedDataAccess"),
        };

        private readonly Lazy<DbProviderFactory> _providerFactory = new Lazy<DbProviderFactory>(() => DbProviderFactoryReflection.GetProviderFactory(OracleDbProviderFactoryDescriptors));

        /// <summary>
        /// The provider factory used for connecting to the database.
        /// </summary>
        /// <remarks>
        /// Searches the <c>OracleClientFactory</c> instance through reflection in order to let the consumer decides
        /// which implementation to use. Supported implementations are from <c>Oracle.ManagedDataAccess.Core</c> (.NET Core) and <c>Oracle.ManagedDataAccess</c> (.NET Framework)
        /// packages.
        /// </remarks>
        /// <exception cref="MissingDbProviderAssemblyException">Neither <c>Oracle.ManagedDataAccess.Core</c> nor <c>Oracle.ManagedDataAccess</c> is referenced.</exception>
        public override DbProviderFactory ProviderFactory => _providerFactory.Value;

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