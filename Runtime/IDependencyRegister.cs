using System;

namespace Damdor.Injectio
{
    public interface IDependencyRegister
    {
        /// <summary>
        /// Registers a dependency instance using its generic type.
        /// </summary>
        /// <typeparam name="T">The type under which the object will be registered.</typeparam>
        /// <param name="obj">The instance to register.</param>
        public void Register<T>(T obj);

        /// <summary>
        /// Registers a dependency instance using the specified type.
        /// </summary>
        /// <param name="type">The type under which the object will be registered.</param>
        /// <param name="obj">The instance to register.</param>
        public void Register(Type type, object obj);

        /// <summary>
        /// Removes a dependency from the container by its type.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        public void Remove(Type type);

        /// <summary>
        /// Removes a dependency from the container by its generic type.
        /// </summary>
        /// <typeparam name="T">The type to remove.</typeparam>
        public void Remove<T>();

        /// <summary>
        /// Clears all registered dependencies from the container.
        /// </summary>
        public void Clear();
    }
}