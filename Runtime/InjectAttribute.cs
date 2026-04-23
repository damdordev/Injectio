using System;

namespace Damdor.Injectio
{
    /// <summary>
    /// Attribute used to mark fields, properties, or methods for dependency injection.
    /// The DependencyResolver will locate these members and inject the appropriate
    /// dependencies from an <see cref="IDependencyContainer"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
        
    }
}