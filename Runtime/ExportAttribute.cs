using System;

namespace Damdor.Injectio
{
    /// <summary>
    /// Attribute used to mark fields or properties for exporting dependencies.
    /// The DependencyExporter will locate these members and register their
    /// values into an <see cref="IDependencyRegister"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExportAttribute : Attribute
    {
        /// <summary>
        /// Gets the explicit type under which the dependency will be registered.
        /// </summary>
        public Type RegisterType { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAttribute"/> class.
        /// </summary>
        public ExportAttribute() : this(null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAttribute"/> class with an explicit registration type.
        /// </summary>
        /// <param name="registerType">The type under which the dependency will be registered.</param>
        public ExportAttribute(Type registerType)
        {
            RegisterType = registerType;
        }
        
    }
}
