namespace DockerRunner.Database.MySql
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