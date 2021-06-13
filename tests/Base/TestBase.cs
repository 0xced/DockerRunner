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

        protected DirectoryInfo TestsDirectory => _testsDirectory.Value;

        protected void RunningCommand(object? sender, CommandEventArgs args)
            => TestOutputHelper.WriteLine($"> {args.Command} {args.Arguments}");

        protected  void RanCommand(object? sender, RanCommandEventArgs args)
            => TestOutputHelper.WriteLine($">> {args.Command} {args.Arguments}{Environment.NewLine}{args.Output.TrimEnd('\n')}");
    }
}