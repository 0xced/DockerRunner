using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DockerRunner.Xunit
{
    /// <summary>
    /// A fixture for xUnit that starts a docker container at initialization time and stops the container at disposal time.
    /// </summary>
    /// <typeparam name="TConfiguration">The <see cref="DockerContainerConfiguration"/> subclass that configures how the docker container is run.</typeparam>
    public class DockerContainerFixture<TConfiguration> : IAsyncLifetime where TConfiguration : DockerContainerConfiguration, new()
    {
        private readonly IMessageSink _messageSink;
        private DockerContainerRunner? _runner;

        /// <summary>
        /// Initialize a new instance of the <see cref="DockerContainerFixture{TConfiguration}"/> class.
        /// </summary>
        /// <param name="messageSink">The <see cref="IMessageSink"/> for writing diagnostics message, i.e. underlying docker commands.</param>
        public DockerContainerFixture(IMessageSink messageSink)
        {
            Configuration = new TConfiguration();
            _messageSink = messageSink ?? throw new ArgumentNullException(nameof(messageSink));
        }

        /// <summary>
        /// The <see cref="DockerContainerConfiguration"/> instance that configures how the docker container is run.
        /// </summary>
        public virtual TConfiguration Configuration { get; }

        async Task IAsyncLifetime.InitializeAsync()
        {
            _runner = await DockerContainerRunner.StartDockerContainerRunnerAsync(Configuration, RunningCommand, RanCommand);
        }

        /// <summary>
        /// The <see cref="DockerRunner.ContainerInfo"/> holding information about the running docker container.
        /// </summary>
        /// <remarks>The container info is only available after the <see cref="IAsyncLifetime.InitializeAsync"/> method has executed.</remarks>
        public virtual ContainerInfo ContainerInfo => _runner?.ContainerInfo ?? throw new InvalidOperationException($"The {nameof(ContainerInfo)} is only available after {nameof(IAsyncLifetime.InitializeAsync)} has executed.");

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_runner != null)
            {
                await _runner.DisposeAsync();
            }
        }

        /// <summary>
        /// Called before a docker command is run.
        /// </summary>
        /// <param name="sender">The <see cref="DockerContainerRunner"/> instance running the command.</param>
        /// <param name="e">The <see cref="CommandEventArgs"/> describing the command.</param>
        protected void RunningCommand(object? sender, CommandEventArgs e)
        {
            _messageSink.OnMessage(new PrintableDiagnosticMessage($"> {e.Command} {e.Arguments}"));
        }

        /// <summary>
        /// Called after a docker command has run.
        /// </summary>
        /// <param name="sender">The <see cref="DockerContainerRunner"/> instance running the command.</param>
        /// <param name="e">The <see cref="CommandEventArgs"/> describing the command.</param>
        protected void RanCommand(object? sender, RanCommandEventArgs e)
        {
            _messageSink.OnMessage(new PrintableDiagnosticMessage($">> {e.Command} {e.Arguments}{Environment.NewLine}{e.Output}"));
        }

        /// <remarks>See https://github.com/xunit/xunit/pull/2148#issuecomment-839838421</remarks>
        private class PrintableDiagnosticMessage : DiagnosticMessage
        {
            public PrintableDiagnosticMessage(string message) : base(message)
            {
            }

            public override string ToString() => Message;
        }
    }
}