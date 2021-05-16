#if DRIVER_MYSQLDATA
namespace DockerRunner.Database.MySql
#elif DRIVER_MYSQLCONNECTOR
namespace DockerRunner.Database.MySqlConnector
#endif
{
    /// <summary>
    /// Configuration for the MySQL Server 5.6 image from https://hub.docker.com/r/mysql/mysql-server
    /// </summary>
    public class MySqlServer56Configuration : MySqlConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mysql/mysql-server:5.6";
    }
}