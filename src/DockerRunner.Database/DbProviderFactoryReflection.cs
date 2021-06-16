using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DockerRunner.Database
{
    /// <summary>
    /// Describes how to get a <see cref="DbProviderFactory"/> instance through reflection.
    /// </summary>
    internal class ProviderFactoryDescriptor
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ProviderFactoryDescriptor"/> class.
        /// </summary>
        /// <param name="className">The fully qualified class name of the <see cref="DbProviderFactory"/>.</param>
        /// <param name="assemblyName">The assembly name where the <see cref="DbProviderFactory"/> can be found.</param>
        /// <param name="nuGetPackageName">The NuGet package name where the <see cref="DbProviderFactory"/> can be found or <see langword="null"/> if the provider factory is built-in.</param>
        /// <param name="instanceFieldName">The name of the public <c>Instance</c> field.</param>
        /// <exception cref="ArgumentNullException"><paramref name="className"/>, <paramref name="assemblyName"/> or <paramref name="instanceFieldName"/> is <see langword="null"/>.</exception>
        public ProviderFactoryDescriptor(string className, string assemblyName, string? nuGetPackageName, string instanceFieldName = "Instance")
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

    internal static class DbProviderFactoryReflection
    {
        /// <summary>
        /// Iterates over the given <paramref name="providerFactoryDescriptors"/> and returns the first
        /// <see cref="DbProviderFactory"/> instance that was successfully loaded through reflection.
        /// </summary>
        /// <param name="providerFactoryDescriptors">A collection of provider factory descriptors that describe how to dynamically load a <see cref="DbProviderFactory"/>.</param>
        /// <returns>The first <see cref="DbProviderFactory"/> that was successfully loaded through reflection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="providerFactoryDescriptors"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="providerFactoryDescriptors"/> is empty.</exception>
        /// <exception cref="MissingFieldException">The <c>Instance</c> field (described by <see cref="ProviderFactoryDescriptor.InstanceFieldName"/>) of the provider factory type is missing.</exception>
        /// <exception cref="MissingAssemblyException">None of the <paramref name="providerFactoryDescriptors"/> could be successfully loaded.</exception>
        public static DbProviderFactory GetProviderFactory(IReadOnlyCollection<ProviderFactoryDescriptor> providerFactoryDescriptors)
        {
            if (providerFactoryDescriptors == null) throw new ArgumentNullException(nameof(providerFactoryDescriptors));
            if (providerFactoryDescriptors.Count == 0) throw new ArgumentException($"The {nameof(providerFactoryDescriptors)} must not be empty.", nameof(providerFactoryDescriptors));

            foreach (var providerFactoryDescriptor in providerFactoryDescriptors)
            {

                var providerFactoryType = Type.GetType(providerFactoryDescriptor.ToString(), throwOnError: false);
                if (providerFactoryType != null)
                {
                    var instanceField = providerFactoryType.GetField(providerFactoryDescriptor.InstanceFieldName);
                    if (instanceField == null)
                    {
                        throw new MissingFieldException(providerFactoryDescriptor.ClassName, providerFactoryDescriptor.InstanceFieldName);
                    }
                    var instance = (DbProviderFactory)instanceField.GetValue(null);
                    return instance;
                }
            }

            var nuGetPackageNames = providerFactoryDescriptors.Select(e => e.NuGetPackageName).Where(e => e != null).Distinct().ToList();
            var packageReferenceDescription = nuGetPackageNames.Count switch
            {
                1 => $"\"{nuGetPackageNames[0]}\"",
                2 => $"either \"{nuGetPackageNames[0]}\" or \"{nuGetPackageNames[1]}\"",
                _ => $"one of {{ \"{string.Join("\", \"", nuGetPackageNames)}\" }}"
            };
            var typesList = providerFactoryDescriptors.Select(e => e.ToString()).Distinct().Select(e => $"  * {e}").ToList();
            var typeNotFoundDescription = typesList.Count == 1 ? "type but it was not found" : "types but none were found";
            var message = $@"Make sure to add a package reference to {packageReferenceDescription} in your project.
Getting a DbProviderFactory instance was attempted with the following assembly-qualified {typeNotFoundDescription}:
{string.Join(Environment.NewLine, typesList)}";
            throw new MissingAssemblyException(message);
        }
    }

    /// <summary>
    /// The exception thrown if <see cref="DbProviderFactoryReflection.GetProviderFactory"/> fails to load a <see cref="DbProviderFactory"/>.
    /// </summary>
    internal class MissingAssemblyException : Exception
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="MissingAssemblyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingAssemblyException(string message) : base(message)
        {
        }
    }
}