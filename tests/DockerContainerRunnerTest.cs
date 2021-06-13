using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Tests
{
    public class DockerContainerRunnerTest : TestBase
    {
        public DockerContainerRunnerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task ReadFileFromNginx()
        {
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
    }
}
