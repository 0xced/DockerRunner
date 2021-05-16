namespace DockerRunner.Database.Npgsql
{
    /// <summary>
    /// Configuration for the latest Postgres 12 image from https://hub.docker.com/_/postgres
    /// </summary>
    public class Postgres12Configuration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:12";
    }
}