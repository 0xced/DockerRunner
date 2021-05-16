namespace DockerRunner.Database.Npgsql
{
    /// <summary>
    /// Configuration for the latest Postgres alpine image from https://hub.docker.com/_/postgres
    /// </summary>
    public class PostgresAlpineConfiguration : PostgresConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "postgres:alpine";
    }
}