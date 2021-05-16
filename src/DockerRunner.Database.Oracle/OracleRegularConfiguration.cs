namespace DockerRunner.Database.Oracle
{
    /// <summary>
    /// Configuration for the latest Oracle Regular image from https://hub.docker.com/r/gvenzl/oracle-xe
    /// </summary>
    public class OracleRegularConfiguration : OracleConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "gvenzl/oracle-xe";

        /// <inheritdoc />
        public override string ServiceName => "XEPDB1";
    }
}