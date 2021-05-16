using DockerRunner.Database.Npgsql;
using Xunit.Abstractions;

namespace DockerRunner.Tests.Npgsql
{
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