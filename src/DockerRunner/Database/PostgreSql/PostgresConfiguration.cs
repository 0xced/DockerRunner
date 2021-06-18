namespace DockerRunner.Database.PostgreSql
{
    /// <summary>
    /// Configuration for the latest Postgres image from https://hub.docker.com/_/postgres
    /// </summary>
    public class PostgresConfiguration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres";
    }
}