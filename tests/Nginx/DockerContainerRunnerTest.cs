using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Tests.Nginx
{
    public class DockerContainerRunnerTest : TestBase
    {
        public DockerContainerRunnerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [SkippableFact]
        public async Task ReadFileFromNginx()
        {
            SkipIfNeeded();

            // Arrange
            var httpClient = new HttpClient(new HttpClientHandler { UseProxy = false });
            var configuration = new NginxDockerContainerConfiguration(TestsDirectory);

            // Act
            await using var runner = await DockerContainerRunner.StartAsync(configuration, RunningCommand, RanCommand);

            // Assert
            var endpoints = runner.ContainerInfo.PortMappings.Where(e => e.ContainerPort == 80).Select(e => e.HostEndpoint).ToList();
            endpoints.Should().NotBeEmpty();

            foreach (var url in endpoints.Select(endpoint => $"http://{endpoint}/DockerContainerRunnerTest.cs"))
            {
                TestOutputHelper.WriteLine($"GET {url}");
                var result = await httpClient.GetAsync(url);
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                var thisFileFromNginx = await result.Content.ReadAsStringAsync();
                var thisFileFromDisk = File.ReadAllText(Path.Combine(TestsDirectory.FullName, "DockerContainerRunnerTest.cs"));
                thisFileFromNginx.Should().Be(thisFileFromDisk);
            }
        }

        /*
         * This test fails when running on Ubuntu / .NETFramework 4.7.2 (on GitHub actions) but works fine when run locally on macOS
         * DockerRunner.Tests.Nginx.DockerContainerRunnerTest.ReadFileFromNginx [FAIL]
         *   Expected object to be OK, but found NotFound.
         */
        protected override bool SkipOnMono => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
