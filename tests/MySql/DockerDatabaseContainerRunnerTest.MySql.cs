using DockerRunner.Database.MySql;
using Xunit.Abstractions;

namespace DockerRunner.Tests.MySql
{
    public class MariaDbTest : DockerDatabaseContainerRunnerTest<MariaDbConfiguration>
    {
        public MariaDbTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
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