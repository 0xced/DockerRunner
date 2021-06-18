namespace DockerRunner.Database.SqlServer
{
    /// <summary>
    /// Configuration for the latest Microsoft SQL Server 2019 image from https://hub.docker.com/_/microsoft-mssql-server
    /// </summary>
    public class SqlServer2019Configuration : SqlServerConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mcr.microsoft.com/mssql/server:2019-latest";
    }
}