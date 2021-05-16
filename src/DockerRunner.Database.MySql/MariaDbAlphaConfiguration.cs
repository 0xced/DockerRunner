#if DRIVER_MYSQLDATA
namespace DockerRunner.Database.MySql
#elif DRIVER_MYSQLCONNECTOR
namespace DockerRunner.Database.MySqlConnector
#endif
{
    /// <summary>
    /// Configuration for the latest MariaDB alpha image from https://hub.docker.com/_/mariadb
    /// </summary>
    public class MariaDbAlphaConfiguration : MySqlConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mariadb:alpha";
    }
}