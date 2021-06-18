namespace DockerRunner.Database.MariaDb
{
    /// <summary>
    /// Configuration for the latest MariaDB 10.2 image from https://hub.docker.com/_/mariadb
    /// </summary>
    public class MariaDb102Configuration : MariaDbConfiguration
    {
        /// <inheritdoc />
        public override string ImageName => "mariadb:10.2";
    }
}