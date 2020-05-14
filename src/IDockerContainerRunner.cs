using System.Threading;
using System.Threading.Tasks;

namespace DockerRunner
{
    /// <summary>
    /// Provides docker container lifecycle management.
    /// </summary>
    public interface IDockerContainerRunner
    {
        /// <summary>
        /// Starts a docker container.
        /// </summary>
        /// <param name="configuration">The <see cref="IDockerContainerConfiguration"/> defining how the container must start.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the start operation. Note that the container may actually continue to start.</param>
        /// <returns>The <see cref="ContainerInfo"/> holding information about the started docker container.</returns>
        Task<ContainerInfo> StartContainerAsync(IDockerContainerConfiguration configuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops a docker container.
        /// </summary>
        /// <param name="containerId">
        /// The <see cref="ContainerId"/> of the container to stop.
        /// This value is found in <see cref="ContainerInfo.ContainerId"/> which is returned when calling <see cref="StartContainerAsync"/>.
        /// </param>
        /// <param name="wait">Returns immediately when <c>false</c> or wait that the container is fully stopped if <c>true</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the stop operation. Has no effect when <paramref name="wait"/> is <c>false</c>.</param>
        Task StopContainerAsync(ContainerId containerId, bool wait, CancellationToken cancellationToken = default);
    }
}