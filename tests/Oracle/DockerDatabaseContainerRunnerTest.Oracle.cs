using DockerRunner.Database.Oracle;
using Xunit.Abstractions;

namespace DockerRunner.Tests.Oracle
{
    public class Oracle11SlimTest : DockerDatabaseContainerRunnerTest<Oracle11SlimConfiguration>
    {
        public Oracle11SlimTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Oracle11RegularTest : DockerDatabaseContainerRunnerTest<Oracle11RegularConfiguration>
    {
        public Oracle11RegularTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Oracle11FullTest : DockerDatabaseContainerRunnerTest<Oracle11FullConfiguration>
    {
        public Oracle11FullTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class OracleRegularTest : DockerDatabaseContainerRunnerTest<OracleRegularConfiguration>
    {
        public OracleRegularTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class OracleFullTest : DockerDatabaseContainerRunnerTest<OracleFullConfiguration>
    {
        public OracleFullTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }
}