#if DRIVER_MYSQLDATA
namespace DockerRunner.Database.MySql
#elif DRIVER_MYSQLCONNECTOR
namespace DockerRunner.Database.MySqlConnector
#endif
{
    /// <summary>
    /// Configuration for the latest MariaDB image from https://hub.docker.com/_/mariadb
    /// </summary>
    public class MariaDbConfiguration : MySqlConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mariadb";
    }
}