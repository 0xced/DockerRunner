using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerRunner
{
    /// <summary>
    /// Configures how to run a docker container.
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
        /// The docker container name.
        /// <para>Defaults to <c>DockerRunner_{fully qualified name of configuration}_{a random guid}</c></para>
        /// <para>Use <see langword="null"/> or an empty string to let Docker assign a random two-words container name.</para>
        /// </summary>
        /// <remarks>The container name must match the following regular expression: <c>[a-zA-Z0-9][a-zA-Z0-9_.-]+</c></remarks>
        public virtual string? ContainerName => $"DockerRunner_{GetType().FullName}_{Guid.NewGuid()}";

        /// <summary>
        /// Automatically remove the container when it exits.
        /// <para>Defaults to <see langword="true"/>.</para>
        /// <para>Use <see langword="false"/> if you need to diagnose a container that doesn't behave as expected after it has exited.</para>
        /// </summary>
        public virtual bool Remove => true;

        /// <summary>
        /// Environment variables to pass to the container.
        /// See the documentation of the docker image for supported environment variables.
        /// </summary>
        public virtual IReadOnlyDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();

        /// <summary>
        /// A list of storage to mount when starting the docker container.
        /// </summary>
        public virtual IEnumerable<DockerStorage> Storage { get; } = Enumerable.Empty<DockerStorage>();

        /// <summary>
        /// Ports to expose when starting the docker container. Useful for images that don't specify an EXPOSE instruction in their Dockerfile.
        /// <para>See https://maximorlov.com/exposing-a-port-in-docker-what-does-it-do/ for a good explanation of exposing ports.</para>
        /// </summary>
        public virtual IEnumerable<ushort> ExposePorts { get; } = Enumerable.Empty<ushort>();
    }
}