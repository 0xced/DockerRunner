namespace DockerRunner.Database.Oracle
{
    /// <summary>
    /// Configuration for the Oracle 11 Regular image from https://hub.docker.com/r/gvenzl/oracle-xe
    /// </summary>
    public class Oracle11RegularConfiguration : OracleConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "gvenzl/oracle-xe:11";
    }
}