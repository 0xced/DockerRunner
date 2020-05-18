using System.Collections.Generic;

namespace DockerRunner
{
    /// <summary>
    /// Configures a docker container.
    /// </summary>
    public interface IDockerContainerConfiguration
    {
        /// <summary>
        /// The docker image name. May include a tag or not.
        /// </summary>
        /// <example>mysql/mysql-server:5.7</example>
        /// <example>redis</example>
        string ImageName { get; }

        /// <summary>
        /// Environment variables to pass to the container.
        /// See the documentation of the docker image for supported environment variables.
        /// </summary>
        IReadOnlyDictionary<string, string> EnvironmentVariables { get; }

        /// <summary>
        /// A list of storage to mount when starting the docker container.
        /// </summary>
        IEnumerable<DockerStorage> Storage { get; }
    }
}