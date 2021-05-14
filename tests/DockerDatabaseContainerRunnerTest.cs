using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Tests
{
    public class DockerDatabaseContainerRunnerTest : TestBase
    {
        public DockerDatabaseContainerRunnerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(typeof(MySqlConfiguration))]
        [InlineData(typeof(OracleConfiguration))]
        [InlineData(typeof(PostgresConfiguration))]
        public async Task StartDockerDatabaseContainer(Type configurationType)
        {
            // Arrange
            var configuration = (DockerDatabaseContainerConfiguration)(Activator.CreateInstance(configurationType) ?? throw new InvalidOperationException($"Activator.CreateInstance({configurationType}) returned null."));

            // Act
            await using var runner = await DockerDatabaseContainerRunner.StartAsync(configuration, RunningCommand, RanCommand);

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
}