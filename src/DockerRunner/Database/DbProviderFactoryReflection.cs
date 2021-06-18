using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DockerRunner.Database
{
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
        /// <exception cref="MissingFieldException">The <c>Instance</c> field (described by <see cref="DbProviderFactoryDescriptor.InstanceFieldName"/>) of the provider factory type is missing.</exception>
        /// <exception cref="MissingDbProviderAssemblyException">None of the <paramref name="providerFactoryDescriptors"/> could be successfully loaded.</exception>
        public static DbProviderFactory GetProviderFactory(IReadOnlyCollection<DbProviderFactoryDescriptor> providerFactoryDescriptors)
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
            throw new MissingDbProviderAssemblyException(message);
        }
    }
}