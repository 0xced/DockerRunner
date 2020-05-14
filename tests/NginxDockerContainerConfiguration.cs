using System;
using System.Collections.Generic;
using System.IO;

namespace DockerRunner.Tests
{
    internal class NginxDockerContainerConfiguration : IDockerContainerConfiguration
    {
        private readonly DirectoryInfo _rootDirectory;

        public NginxDockerContainerConfiguration(DirectoryInfo rootDirectory)
        {
            _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
        }

        /// <summary>
        /// Image name of the official build of nginx, see https://hub.docker.com/_/nginx
        /// </summary>
        public string ImageName => "nginx";

        /// <summary>
        /// No environment variables are required for the docker nginx docker image.
        /// </summary>
        public IReadOnlyDictionary<string, string> EnvironmentVariables => new Dictionary<string, string>();

        /// <summary>
        /// A single bind mount to serve documents on the host machine through nginx.
        /// </summary>
        public IEnumerable<DockerStorage> Storage => new[] { new DockerBindMount(_rootDirectory, "/usr/share/nginx/html", isReadOnly: true)  };
    }
}