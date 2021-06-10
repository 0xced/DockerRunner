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
            // Arrange
            var configuration = CreateConfiguration();

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

        private static TConfiguration CreateConfiguration()
        {
            var configuration = new TConfiguration();

            var isRunningOnGitHubActions = bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var result) && result;

            // Oracle tests fail in many ways when running GitHub Actions
            // * OracleException ORA-01882: timezone region not found
            // * Running the command `docker pull gvenzl/oracle-xe:full` failed with exit code 1 and produced this error on stderr: failed to register layer: ApplyLayer exit status 1 stdout:  stderr: write /opt/oracle/product/18c/dbhomeXE/lib/libmkl_intel_ilp64.so: no space left on device
            Skip.If(isRunningOnGitHubActions && configuration is Database.Oracle.OracleConfigurationBase, "Oracle test would fail when running on GitHub Actions.");

            // This consistently fails with the following exception:
            // System.TimeoutException : Database was not available on "Server=127.0.0.1;Port=49211;Database=database;User Id=root;***" after waiting for 30.0 seconds.
            // ---- MySqlConnector.MySqlException : SSL Authentication Error
            // -------- System.Security.Authentication.AuthenticationException : Authentication failed, see inner exception.
            // ------------ Interop+OpenSsl+SslException : SSL Handshake failed with OpenSSL error - SSL_ERROR_SSL.
            // ---------------- Interop+Crypto+OpenSslCryptographicException : error:14094410:SSL routines:ssl3_read_bytes:sslv3 alert handshake failure
            var isMySqlServer57 = configuration is Database.MySqlConnector.MySqlServer57Configuration || configuration is Database.MySql.MySqlServer57Configuration;
            Skip.If(isRunningOnGitHubActions && isMySqlServer57, "MySQL Server 5.7 test would fail when running on GitHub Actions.");

            return configuration;
        }
    }
}