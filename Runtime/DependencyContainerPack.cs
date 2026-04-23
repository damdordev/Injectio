using System;
using System.Collections.Generic;

namespace Damdor.Injectio
{
    /// <summary>
    /// A composite dependency container that holds multiple <see cref="IDependencyContainer"/> instances.
    /// When querying for a dependency, it iterates through its registered containers until it finds a match.
    /// <remarks> Dependency container added last has the highest priority. </remarks>
    /// </summary>
    public class DependencyContainerPack : IDependencyContainer
    {
        private readonly List<IDependencyContainer> containers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContainerPack"/> class.
        /// </summary>
        public DependencyContainerPack() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContainerPack"/> class with the specified containers.
        /// </summary>
        /// <param name="containers">An array of containers to initialize the pack with.</param>
        public DependencyContainerPack(params IDependencyContainer[] containers)
        {
            this.containers.Clear();
            this.containers.AddRange(containers);
        }

        /// <summary>
        /// Clears all registered containers from the pack.
        /// </summary>
        public void Clear() => containers.Clear();
        
        /// <summary>
        /// Replaces the current containers with a new sequence of containers.
        /// </summary>
        /// <param name="containers">The new containers to use.</param>
        public void Setup(IEnumerable<IDependencyContainer> containers)
        {
            this.containers.Clear();
            this.containers.AddRange(containers);
        }

        /// <summary>
        /// Adds a single container to the pack.
        /// </summary>
        /// <param name="container">The container to add.</param>
        public void Add(IDependencyContainer container) => containers.Add(container);

        /// <summary>
        /// Adds multiple containers to the pack.
        /// </summary>
        /// <param name="containers">The sequence of containers to add.</param>
        public void Add(IEnumerable<IDependencyContainer> containers) => this.containers.AddRange(containers);

        /// <summary>
        /// Adds multiple containers to the pack.
        /// </summary>
        /// <param name="containers">An array of containers to add.</param>
        public void Add(params IDependencyContainer[] containers) => this.containers.AddRange(containers);

        /// <summary>
        /// Iterates through the registered containers to retrieve a dependency of the specified type.
        /// Returns the first non-null dependency found.
        /// </summary>
        /// <param name="type">The type of the dependency to resolve.</param>
        /// <returns>The resolved dependency object, or null if no container holds the requested type.</returns>
        public object Get(Type type)
        {
            for (var index = containers.Count - 1; index >= 0; index--)
            {
                var container = containers[index];
                var result = container.Get(type);
                if (result != null) return result;
            }

            return null;
        }
    }
}