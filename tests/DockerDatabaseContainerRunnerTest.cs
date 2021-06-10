using System;
using System.Threading.Tasks;
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

        [SkippableFact]
        public async Task StartDockerDatabaseContainer()
        {
            // Arrange
            var configuration = new TConfiguration();

            // Oracle tests fail in many ways when running GitHub Actions
            // * OracleException ORA-01882: timezone region not found
            // * Running the command `docker pull gvenzl/oracle-xe:full` failed with exit code 1 and produced this error on stderr: failed to register layer: ApplyLayer exit status 1 stdout:  stderr: write /opt/oracle/product/18c/dbhomeXE/lib/libmkl_intel_ilp64.so: no space left on device
            Skip.If(IsRunningOnGitHubActions() && configuration is OracleConfigurationBase, "Oracle test would fail when running on GitHub Actions.");

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

        private static bool IsRunningOnGitHubActions() => bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var result) && result;
    }
}