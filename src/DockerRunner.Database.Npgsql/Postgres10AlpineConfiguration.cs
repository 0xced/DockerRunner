namespace DockerRunner.Database.Npgsql
{
    /// <summary>
    /// Configuration for the latest Postgres 10 alpine image from https://hub.docker.com/_/postgres
    /// </summary>
    public class Postgres10AlpineConfiguration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:10-alpine";
    }
}