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
            var configuration = new NginxDockerContainerConfiguration(TestsDirectory);
            await using var runner = await DockerContainerRunner.StartAsync(configuration, RunningCommand, RanCommand);
            var containerInfo = runner.ContainerInfo;
            var httpClient = new HttpClient();
            var endpoints = containerInfo.PortMappings.Where(e => e.ContainerPort == 80).Select(e => e.HostEndpoint).ToList();
            endpoints.Should().NotBeEmpty();
            foreach (var url in endpoints.Select(endpoint => $"http://{endpoint}/DockerContainerRunnerTest.cs"))
            {
                TestOutputHelper.WriteLine($"GET {url}");
                var result = await httpClient.GetAsync(url);
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                var thisFileFromNginx = await result.Content.ReadAsStringAsync();
                var thisFileFromDisk = await File.ReadAllTextAsync(Path.Combine(TestsDirectory.FullName, "DockerContainerRunnerTest.cs"));
                thisFileFromNginx.Should().Be(thisFileFromDisk);
            }
        }
    }
}
