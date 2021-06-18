using System;
using System.Data.Common;

namespace DockerRunner.Database
{
    /// <summary>
    /// Describes how to get a <see cref="DbProviderFactory"/> instance through reflection.
    /// </summary>
    internal class DbProviderFactoryDescriptor
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="DbProviderFactoryDescriptor"/> class.
        /// </summary>
        /// <param name="className">The fully qualified class name of the <see cref="DbProviderFactory"/>.</param>
        /// <param name="assemblyName">The assembly name where the <see cref="DbProviderFactory"/> can be found.</param>
        /// <param name="nuGetPackageName">The NuGet package name where the <see cref="DbProviderFactory"/> can be found or <see langword="null"/> if the provider factory is built-in.</param>
        /// <param name="instanceFieldName">The name of the public <c>Instance</c> field.</param>
        /// <exception cref="ArgumentNullException"><paramref name="className"/>, <paramref name="assemblyName"/> or <paramref name="instanceFieldName"/> is <see langword="null"/>.</exception>
        public DbProviderFactoryDescriptor(string className, string assemblyName, string? nuGetPackageName, string instanceFieldName = "Instance")
        {
            ClassName = className ?? throw new ArgumentNullException(nameof(className));
            AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
            NuGetPackageName = nuGetPackageName;
            InstanceFieldName = instanceFieldName ?? throw new ArgumentNullException(nameof(instanceFieldName));
        }

        /// <summary>
        /// The fully qualified class name of the <see cref="DbProviderFactory"/>.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The assembly name where the <see cref="DbProviderFactory"/> can be found.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The NuGet package name where the <see cref="DbProviderFactory"/> can be found
        /// or <see langword="null"/> if the provider factory is built-in.
        /// </summary>
        public string? NuGetPackageName { get; }

        /// <summary>
        /// The name of the public <c>Instance</c> field.
        /// </summary>
        public string InstanceFieldName { get; }

        /// <returns>The assembly-qualified name representing the <see cref="DbProviderFactory"/> class, suitable for <see cref="Type.GetType(string)"/>.</returns>
        public override string ToString() => $"{ClassName}, {AssemblyName}";
    }
}