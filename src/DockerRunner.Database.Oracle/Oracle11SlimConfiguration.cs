namespace DockerRunner.Database.Oracle
{
    /// <summary>
    /// Configuration for the Oracle 11 Slim image from https://hub.docker.com/r/gvenzl/oracle-xe
    /// </summary>
    public class Oracle11SlimConfiguration : OracleConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "gvenzl/oracle-xe:11-slim";
    }
}