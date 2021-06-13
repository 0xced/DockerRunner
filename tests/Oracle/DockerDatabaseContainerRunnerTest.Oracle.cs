using DockerRunner.Database.Oracle;
using Xunit.Abstractions;

namespace DockerRunner.Tests.Oracle
{
    public abstract class OracleTest<T> : DockerDatabaseContainerRunnerTest<T> where T : DockerDatabaseContainerConfiguration, new()
    {
        protected OracleTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        // Oracle tests fail in many ways when running GitHub Actions
        // * OracleException ORA-01882: timezone region not found
        // * Running the command `docker pull gvenzl/oracle-xe:full` failed with exit code 1 and produced this error on stderr: failed to register layer: ApplyLayer exit status 1 stdout:  stderr: write /opt/oracle/product/18c/dbhomeXE/lib/libmkl_intel_ilp64.so: no space left on device
        protected override bool SkipOnGitHubActions => true;
    }

    public class Oracle11SlimTest : OracleTest<Oracle11SlimConfiguration>
    {
        public Oracle11SlimTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Oracle11RegularTest : OracleTest<Oracle11RegularConfiguration>
    {
        public Oracle11RegularTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Oracle11FullTest : OracleTest<Oracle11FullConfiguration>
    {
        public Oracle11FullTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class OracleRegularTest : OracleTest<OracleRegularConfiguration>
    {
        public OracleRegularTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class OracleFullTest : OracleTest<OracleFullConfiguration>
    {
        public OracleFullTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }
}