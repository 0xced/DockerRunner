namespace DockerRunner.Database.Npgsql
{
    /// <summary>
    /// Configuration for the latest Postgres 12 alpine image from https://hub.docker.com/_/postgres
    /// </summary>
    public class Postgres12AlpineConfiguration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:12-alpine";
    }
}