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
            var isRunningOnMono = Type.GetType("Mono.Runtime") != null;
            Skip.If(isRunningOnMono && SkipOnMono, "Test would fail when running on Mono.");

            var isRunningOnGitHubActions = bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var result) && result;
            Skip.If(isRunningOnGitHubActions && SkipOnGitHubActions, "Test would fail when running on GitHub Actions.");

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

        protected virtual bool SkipOnMono => false;

        protected virtual bool SkipOnGitHubActions => false;
    }
}
