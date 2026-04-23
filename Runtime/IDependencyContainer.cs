using System;

namespace Damdor.Injectio
{
    /// <summary>
    /// Represents a container that provides dependencies by their type.
    /// Can be queried by <see cref="DependencyResolver"/> to satisfy <see cref="InjectAttribute"/>.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Retrieves a dependency of the specified type from the container.
        /// </summary>
        /// <param name="type">The type of the dependency to resolve.</param>
        /// <returns>The resolved dependency object, or null if it cannot be found.</returns>
        object Get(Type type);
    }
}