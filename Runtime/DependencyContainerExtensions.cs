namespace Damdor.Injectio
{
    /// <summary>
    /// Extension methods for <see cref="IDependencyContainer"/>.
    /// </summary>
    public static class DependencyContainerExtensions
    {
        /// <summary>
        /// Retrieves a registered dependency of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type of the dependency to resolve.</typeparam>
        /// <param name="container">The dependency container instance.</param>
        /// <returns>The resolved dependency object, or null if it cannot be found.</returns>
        public static T Get<T>(this IDependencyContainer container) where T : class
        {
            return (T)container.Get(typeof(T));
        }
    }
}