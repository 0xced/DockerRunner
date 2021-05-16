namespace DockerRunner.Database.MySql
{
    /// <summary>
    /// Configuration for the MySQL Server 5.7 image from https://hub.docker.com/r/mysql/mysql-server
    /// </summary>
    public class MySqlServer57Configuration : MySqlConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "mysql/mysql-server:5.7";
    }
}