using System;
using System.Collections.Generic;
using System.IO;

namespace DockerRunner.Tests
{
    internal class NginxDockerContainerConfiguration : DockerContainerConfiguration
    {
        private readonly DirectoryInfo _rootDirectory;

        public NginxDockerContainerConfiguration(DirectoryInfo rootDirectory)
        {
            _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
        }

        /// <summary>
        /// Image name of the official build of nginx, see https://hub.docker.com/_/nginx
        /// </summary>
        public override string ImageName => "nginx";

        /// <summary>
        /// A single bind mount to serve documents on the host machine through nginx.
        /// </summary>
        public override IEnumerable<DockerStorage> Storage => new[] { new DockerBindMount(_rootDirectory, "/usr/share/nginx/html", isReadOnly: true)  };
    }
}