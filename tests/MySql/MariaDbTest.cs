using DockerRunner.Database.MariaDb;
using Xunit.Abstractions;

namespace DockerRunner.Tests.MySql
{
    public class MariaDbAlphaTest : DockerDatabaseContainerRunnerTest<MariaDbAlphaConfiguration>
    {
        public MariaDbAlphaTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDbTest : DockerDatabaseContainerRunnerTest<MariaDbConfiguration>
    {
        public MariaDbTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDb104Test : DockerDatabaseContainerRunnerTest<MariaDb104Configuration>
    {
        public MariaDb104Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDb103Test : DockerDatabaseContainerRunnerTest<MariaDb103Configuration>
    {
        public MariaDb103Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDb102Test : DockerDatabaseContainerRunnerTest<MariaDb102Configuration>
    {
        public MariaDb102Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }
}