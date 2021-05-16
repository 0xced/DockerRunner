using DockerRunner.Database.SqlServer;
using Xunit.Abstractions;

namespace DockerRunner.Tests.SqlServer
{
    public class SqlServer2017Test : DockerDatabaseContainerRunnerTest<SqlServer2017Configuration>
    {
        public SqlServer2017Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class SqlServer2019Test : DockerDatabaseContainerRunnerTest<SqlServer2019Configuration>
    {
        public SqlServer2019Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }
}