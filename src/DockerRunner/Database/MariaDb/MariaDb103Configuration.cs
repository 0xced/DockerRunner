namespace DockerRunner.Database.MariaDb
{
    /// <summary>
    /// Configuration for the latest MariaDB 10.3 image from https://hub.docker.com/_/mariadb
    /// </summary>
    public class MariaDb103Configuration : MariaDbConfiguration
    {
        /// <inheritdoc />
        public override string ImageName => "mariadb:10.3";
    }
}