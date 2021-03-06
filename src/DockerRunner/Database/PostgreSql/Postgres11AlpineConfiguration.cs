namespace DockerRunner.Database.PostgreSql
{
    /// <summary>
    /// Configuration for the latest Postgres 11 alpine image from https://hub.docker.com/_/postgres
    /// </summary>
    public class Postgres11AlpineConfiguration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:11-alpine";
    }
}