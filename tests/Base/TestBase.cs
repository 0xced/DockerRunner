using System;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

// Put all test classes into a single test collection, thus disabling parallel testing
// See https://xunit.net/docs/running-tests-in-parallel
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace DockerRunner.Tests
{
    public abstract class TestBase
    {
        protected readonly ITestOutputHelper TestOutputHelper;

        private readonly Lazy<DirectoryInfo> _testsDirectory = new Lazy<DirectoryInfo>(() =>
        {
            var assemblyDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            var targetFrameworkDirectory = assemblyDirectory?.Name == "publish" ? assemblyDirectory.Parent?.Parent : assemblyDirectory;
            var testsDirectoryInfo = targetFrameworkDirectory?.Parent?.Parent?.Parent ?? throw new FileNotFoundException("Tests directory not found");
            return testsDirectoryInfo;
        });

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        protected void SkipIfNeeded()
        {
            var isRunningOnMono = Type.GetType("Mono.Runtime") != null;
            Skip.If(isRunningOnMono && SkipOnMono, "Test would fail when running on Mono.");

            var isRunningOnGitHubActions = bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var result) && result;
            Skip.If(isRunningOnGitHubActions && SkipOnGitHubActions, "Test would fail when running on GitHub Actions.");
        }

        protected virtual bool SkipOnMono => false;

        protected virtual bool SkipOnGitHubActions => false;

        protected DirectoryInfo TestsDirectory => _testsDirectory.Value;

        protected void RunningCommand(object? sender, CommandEventArgs args)
            => TestOutputHelper.WriteLine($"> {args.Command} {args.Arguments}");

        protected  void RanCommand(object? sender, RanCommandEventArgs args)
            => TestOutputHelper.WriteLine($">> {args.Command} {args.Arguments}{Environment.NewLine}{args.Output.TrimEnd('\n')}");
    }
}