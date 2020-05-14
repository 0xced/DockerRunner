using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Tests
{
    public class DockerContainerRunnerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DockerContainerRunnerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact]
        public async Task ReadFileFromNginx()
        {
            var testsDirectory = GetTestsDirectory();
            var configuration = new NginxDockerContainerConfiguration(testsDirectory);
#if NETCOREAPP2_1
            var runner = await DockerContainerRunner.StartDockerContainerRunnerAsync(configuration, RunningCommand, RanCommand);
            await runner.DisposeAsync();
#else
            await using var runner = await DockerContainerRunner.StartDockerContainerRunnerAsync(configuration, RunningCommand, RanCommand);
#endif
            var containerInfo = runner.ContainerInfo;
            var httpClient = new HttpClient();
            var host = containerInfo.Host;
            var port = containerInfo.PortMappings.Single(e => e.ContainerPort == 80).HostPort;
            var result = await httpClient.GetAsync($"http://{host}:{port}/DockerContainerRunnerTest.cs");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var thisFileFromNginx = await result.Content.ReadAsStringAsync();
            var thisFileFromDisk = await File.ReadAllTextAsync(Path.Combine(testsDirectory.FullName, "DockerContainerRunnerTest.cs"));
            thisFileFromNginx.Should().Be(thisFileFromDisk);
        }

        private static DirectoryInfo GetTestsDirectory()
        {
            var assemblyDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            var targetFrameworkDirectory = assemblyDirectory?.Name == "publish" ? assemblyDirectory.Parent?.Parent : assemblyDirectory;
            var testsDirectoryInfo = targetFrameworkDirectory?.Parent?.Parent?.Parent ?? throw new FileNotFoundException("Tests directory not found");
            return testsDirectoryInfo;
        }

        private void RunningCommand(object? sender, CommandEventArgs args)
            => _testOutputHelper.WriteLine($"> {args.Command} {args.Arguments}");
        private void RanCommand(object? sender, RanCommandEventArgs args)
            => _testOutputHelper.WriteLine($">> {args.Command} {args.Arguments}{Environment.NewLine}{args.Output.TrimEnd('\n')}");
    }
}
