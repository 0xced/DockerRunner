namespace DockerRunner.Database.Oracle
{
    /// <summary>
    /// Configuration for the Oracle 11 Full image from https://hub.docker.com/r/gvenzl/oracle-xe
    /// </summary>
    public class Oracle11FullConfiguration : OracleConfigurationBase
    {
        /// <inheritdoc />
        public override string ImageName => "gvenzl/oracle-xe:11-full";

        /// <inheritdoc />
        public override string ServiceName => "XE";
    }
}