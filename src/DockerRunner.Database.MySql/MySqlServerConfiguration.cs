#if DRIVER_MYSQLDATA
namespace DockerRunner.Database.MySql
#elif DRIVER_MYSQLCONNECTOR
namespace DockerRunner.Database.MySqlConnector
#endif
{
    /// <summary>
    /// Configuration for the latest MySQL Server image from https://hub.docker.com/r/mysql/mysql-server
    /// </summary>
    public class MySqlServerConfiguration : MySqlConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mysql/mysql-server";
    }
}