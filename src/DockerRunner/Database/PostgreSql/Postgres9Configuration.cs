namespace DockerRunner.Database.PostgreSql
{
    /// <summary>
    /// Configuration for the latest Postgres 9 image from https://hub.docker.com/_/postgres
    /// </summary>
    public class Postgres9Configuration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:9";
    }
}