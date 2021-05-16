using System.Threading.Tasks;
using DockerRunner.Database.MySql;
using DockerRunner.Database.Npgsql;
using DockerRunner.Database.Oracle;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Tests
{
    public abstract class DockerDatabaseContainerRunnerTest<TConfiguration> : TestBase
        where TConfiguration : DockerDatabaseContainerConfiguration, new()
    {
        public DockerDatabaseContainerRunnerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        [Fact]
        public async Task StartDockerDatabaseContainer()
        {
            // Arrange
            var configuration = new TConfiguration();

            // Act
            await using var runner = await DockerDatabaseContainerRunner.StartAsync(configuration, RunningCommand, RanCommand, waitOnDispose: true);

            // Assert
            runner.ContainerInfo.ContainerId.Should().NotBeNull();
            runner.ContainerInfo.PortMappings.Should().NotBeEmpty();

            TestOutputHelper.WriteLine($"ConnectionString: {runner.ConnectionString}");
            TestOutputHelper.WriteLine($"ContainerId: {runner.ContainerInfo.ContainerId}");
            foreach (var portMapping in runner.ContainerInfo.PortMappings)
            {
                TestOutputHelper.WriteLine($"  PortMapping: {portMapping.HostEndpoint} -> {portMapping.ContainerPort}");
            }
        }
    }

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

    public class MySqlConnectorTest : DockerDatabaseContainerRunnerTest<MySqlConnectorConfiguration>
    {
        public MySqlConnectorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

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

    public class Postgres9Test : DockerDatabaseContainerRunnerTest<Postgres9Configuration>
    {
        public Postgres9Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres10Test : DockerDatabaseContainerRunnerTest<Postgres10Configuration>
    {
        public Postgres10Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres11Test : DockerDatabaseContainerRunnerTest<Postgres11Configuration>
    {
        public Postgres11Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres12Test : DockerDatabaseContainerRunnerTest<Postgres12Configuration>
    {
        public Postgres12Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class PostgresTest : DockerDatabaseContainerRunnerTest<PostgresConfiguration>
    {
        public PostgresTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres9AlpineTest : DockerDatabaseContainerRunnerTest<Postgres9AlpineConfiguration>
    {
        public Postgres9AlpineTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres10AlpineTest : DockerDatabaseContainerRunnerTest<Postgres10AlpineConfiguration>
    {
        public Postgres10AlpineTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres11AlpineTest : DockerDatabaseContainerRunnerTest<Postgres11AlpineConfiguration>
    {
        public Postgres11AlpineTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class Postgres12AlpineTest : DockerDatabaseContainerRunnerTest<Postgres12AlpineConfiguration>
    {
        public Postgres12AlpineTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class PostgresAlpineTest : DockerDatabaseContainerRunnerTest<PostgresAlpineConfiguration>
    {
        public PostgresAlpineTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }
}