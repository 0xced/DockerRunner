using System.Collections.Generic;
using System.Linq;

namespace DockerRunner
{
    /// <summary>
    /// Configures a docker container.
    /// </summary>
    public abstract class DockerContainerConfiguration
    {
        /// <summary>
        /// The docker image name. May include a tag or not.
        /// </summary>
        /// <example>mysql/mysql-server:5.7</example>
        /// <example>redis</example>
        public abstract string ImageName { get; }

        /// <summary>
        /// Environment variables to pass to the container.
        /// See the documentation of the docker image for supported environment variables.
        /// </summary>
        public virtual IReadOnlyDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();

        /// <summary>
        /// A list of storage to mount when starting the docker container.
        /// </summary>
        public virtual IEnumerable<DockerStorage> Storage { get; } = Enumerable.Empty<DockerStorage>();
    }
}