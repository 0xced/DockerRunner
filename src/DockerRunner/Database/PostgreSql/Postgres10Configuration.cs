namespace DockerRunner.Database.PostgreSql
{
    /// <summary>
    /// Configuration for the latest Postgres 10 image from https://hub.docker.com/_/postgres
    /// </summary>
    public class Postgres10Configuration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:10";
    }
}