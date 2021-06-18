using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Tests
{
    public abstract class DockerDatabaseContainerRunnerTest<TConfiguration> : TestBase
        where TConfiguration : DockerDatabaseContainerConfiguration, new()
    {
        public DockerDatabaseContainerRunnerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        [SkippableFact]
        public async Task StartDockerDatabaseContainer()
        {
            SkipIfNeeded();

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
}
