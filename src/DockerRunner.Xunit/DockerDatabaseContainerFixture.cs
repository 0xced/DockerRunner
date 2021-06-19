using System;
using System.Data.Common;
using System.Threading.Tasks;
using DockerRunner.Database;
using Xunit;
using Xunit.Abstractions;

namespace DockerRunner.Xunit
{
    /// <summary>
    /// A fixture for xUnit that starts a docker database container at initialization time and stops the container at disposal time.
    /// The fixture waits for the database to become available after the docker container has started.
    /// </summary>
    /// <typeparam name="TConfiguration">The <see cref="DockerDatabaseContainerConfiguration"/> subclass that configures how the docker database container is run.</typeparam>
    public class DockerDatabaseContainerFixture<TConfiguration> : DockerContainerFixture<TConfiguration>, IAsyncLifetime where TConfiguration : DockerDatabaseContainerConfiguration, new()
    {
        private DockerDatabaseContainerRunner? _runner;

        /// <summary>
        /// Initialize a new instance of the <see cref="DockerDatabaseContainerFixture{TConfiguration}"/> class.
        /// </summary>
        /// <param name="messageSink">The <see cref="IMessageSink"/> for writing diagnostics message, i.e. underlying docker commands.</param>
        public DockerDatabaseContainerFixture(IMessageSink messageSink) : base(messageSink)
        {
            Configuration = new TConfiguration();
        }

        /// <summary>
        /// The <see cref="DockerDatabaseContainerConfiguration"/> instance that configures how the docker container is run.
        /// </summary>
        public override TConfiguration Configuration { get; }

        async Task IAsyncLifetime.InitializeAsync()
        {
            _runner = await DockerDatabaseContainerRunner.StartAsync(Configuration, RunningCommand, RanCommand, SqlCommandExecuting);
        }

        /// <inheritdoc />
        public override ContainerInfo ContainerInfo => _runner?.ContainerInfo ?? throw new InvalidOperationException($"The {nameof(ContainerInfo)} is only available after {nameof(IAsyncLifetime.InitializeAsync)} has executed.");

        /// <summary>
        /// The connection string to use to connect to the database.
        /// </summary>
        /// <remarks>The connection string is only available after the <see cref="IAsyncLifetime.InitializeAsync"/> method has executed.</remarks>
        public string ConnectionString => _runner?.ConnectionString ?? throw new InvalidOperationException($"The {nameof(ConnectionString)} is only available after {nameof(IAsyncLifetime.InitializeAsync)} has executed.");

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_runner != null)
            {
                await _runner.DisposeAsync();
            }
        }

        /// <summary>
        /// Called before a <see cref="DbCommand" /> is run as part of <see cref="DockerDatabaseContainerRunner"/> startup.
        /// </summary>
        /// <param name="sender">The <see cref="DockerDatabaseContainerRunner"/> instance executing the db command.</param>
        /// <param name="e">The <see cref="DbCommandEventArgs"/> holding the db command.</param>
        protected void SqlCommandExecuting(object? sender, DbCommandEventArgs e)
        {
            MessageSink.OnMessage(new PrintableDiagnosticMessage($"Executing DbCommand ({e.DbCommand.CommandType}){Environment.NewLine}{e.DbCommand.CommandText}"));
        }
    }
}