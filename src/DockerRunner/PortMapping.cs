using System.Collections.Generic;
using System.Net.Sockets;

namespace DockerRunner
{
    /// <summary>
    /// Port mapping between the host machine and the docker container.
    /// </summary>
    public class PortMapping
    {
        /// <summary>
        /// An equality comparer that compares all properties of a <see cref="PortMapping"/> instance,
        /// i.e. <see cref="HostPort"/>, <see cref="ContainerPort"/> and <see cref="AddressFamily"/>
        /// </summary>
        public static IEqualityComparer<PortMapping> Comparer { get; } = new PortMappingEqualityComparer();

        /// <summary>
        /// Initialize a new instance of the <see cref="PortMapping"/> class.
        /// </summary>
        /// <param name="hostPort">The port on the host machine.</param>
        /// <param name="containerPort">The port exposed by the docker container.</param>
        /// <param name="addressFamily">The <see cref="AddressFamily"/> associated to the host port.</param>
        public PortMapping(ushort hostPort, ushort containerPort, AddressFamily addressFamily)
        {
            HostPort = hostPort;
            ContainerPort = containerPort;
            AddressFamily = addressFamily;
        }

        /// <summary>
        /// The port on the host machine.
        /// </summary>
        public ushort HostPort { get; }

        /// <summary>
        /// The port exposed by the docker container.
        /// </summary>
        public ushort ContainerPort { get; }

        /// <summary>
        /// The <see cref="AddressFamily"/> associated to the host port.
        /// </summary>
        public AddressFamily AddressFamily { get; }

        private sealed class PortMappingEqualityComparer : IEqualityComparer<PortMapping>
        {
            public bool Equals(PortMapping? x, PortMapping? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.HostPort == y.HostPort && x.ContainerPort == y.ContainerPort && x.AddressFamily == y.AddressFamily;
            }

            public int GetHashCode(PortMapping obj)
            {
                unchecked
                {
                    var hashCode = obj.HostPort.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ContainerPort.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int) obj.AddressFamily;
                    return hashCode;
                }
            }
        }
    }
}