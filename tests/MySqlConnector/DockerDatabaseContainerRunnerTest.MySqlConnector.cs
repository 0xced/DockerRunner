using DockerRunner.Database.MySqlConnector;
using Xunit.Abstractions;

namespace DockerRunner.Tests.MySqlConnector
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

    public class MySqlServerTest : DockerDatabaseContainerRunnerTest<MySqlServerConfiguration>
    {
        public MySqlServerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MySqlServer56Test : DockerDatabaseContainerRunnerTest<MySqlServer56Configuration>
    {
        public MySqlServer56Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MySqlServer57Test : DockerDatabaseContainerRunnerTest<MySqlServer57Configuration>
    {
        public MySqlServer57Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }
}