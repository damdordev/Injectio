using System;
using System.Collections.Generic;

namespace Damdor.Injectio
{
    /// <summary>
    /// A simple, dictionary-based implementation of <see cref="IDependencyContainer"/>.
    /// Allows registering, removing, and retrieving instances by their type.
    /// Note: Does not support multithreading due to performance reasons.
    /// </summary>
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, object> dependencies = new();

        /// <summary>
        /// Retrieves a registered dependency of the specified type.
        /// </summary>
        /// <param name="type">The type of the dependency.</param>
        /// <returns>The registered object, or null if the type is not registered.</returns>
        public object Get(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            dependencies.TryGetValue(type, out var obj);
            return obj;
        }
        
        /// <summary>
        /// Registers a dependency instance using its generic type.
        /// </summary>
        /// <typeparam name="T">The type under which the object will be registered.</typeparam>
        /// <param name="obj">The instance to register.</param>
        public void Register<T>(T obj) => dependencies[typeof(T)] = obj;

        /// <summary>
        /// Registers a dependency instance using the specified type.
        /// </summary>
        /// <param name="type">The type under which the object will be registered.</param>
        /// <param name="obj">The instance to register.</param>
        public void Register(Type type, object obj) => dependencies[type] = obj;

        /// <summary>
        /// Removes a dependency from the container by its type.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        public void Remove(Type type) => dependencies.Remove(type);

        /// <summary>
        /// Removes a dependency from the container by its generic type.
        /// </summary>
        /// <typeparam name="T">The type to remove.</typeparam>
        public void Remove<T>() => dependencies.Remove(typeof(T));
        
        /// <summary>
        /// Clears all registered dependencies from the container.
        /// </summary>
        public void Clear()
        {
            dependencies.Clear();
        }
    }
}