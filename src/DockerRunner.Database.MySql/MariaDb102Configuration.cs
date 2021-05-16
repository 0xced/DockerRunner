#if DRIVER_MYSQLDATA
namespace DockerRunner.Database.MySql
#elif DRIVER_MYSQLCONNECTOR
namespace DockerRunner.Database.MySqlConnector
#endif
{
    /// <summary>
    /// Configuration for the latest MariaDB 10.2 image from https://hub.docker.com/_/mariadb
    /// </summary>
    public class MariaDb102Configuration : MySqlConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mariadb:10.2";
    }
}