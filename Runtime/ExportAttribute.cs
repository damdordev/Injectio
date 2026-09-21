using System;

namespace Damdor.Injectio
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExportAttribute : Attribute
    {
        public Type RegisterType { get; }
        
        public ExportAttribute() : this(null) {}

        public ExportAttribute(Type registerType)
        {
            RegisterType = registerType;
        }
        
    }
}