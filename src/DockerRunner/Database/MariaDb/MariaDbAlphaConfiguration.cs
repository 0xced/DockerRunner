namespace DockerRunner.Database.MariaDb
{
    /// <summary>
    /// Configuration for the latest MariaDB alpha image from https://hub.docker.com/_/mariadb
    /// </summary>
    public class MariaDbAlphaConfiguration : MariaDbConfiguration
    {
        /// <inheritdoc />
        public override string ImageName => "mariadb:alpha";
    }
}