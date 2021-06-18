namespace DockerRunner.Database.Oracle
{
    /// <summary>
    /// Configuration for the latest Oracle Full image from https://hub.docker.com/r/gvenzl/oracle-xe
    /// </summary>
    public class OracleFullConfiguration : OracleConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "gvenzl/oracle-xe:full";

        /// <inheritdoc />
        public override string ServiceName => "XEPDB1";
    }
}