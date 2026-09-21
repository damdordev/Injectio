using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Damdor.Injectio
{
    public class DependencyExporterFieldData
    {
        public FieldInfo Field { get; }
        public ExportAttribute Attribute { get; }
        
        public DependencyExporterFieldData(FieldInfo field, ExportAttribute attribute)
        {
            Field = field;
            Attribute = attribute;
        }
    }

    public class DependencyExporterPropertyData
    {
        public PropertyInfo Property { get; }
        public ExportAttribute Attribute { get; }

        public DependencyExporterPropertyData(PropertyInfo property, ExportAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }
    }

    public class DependencyExporterTypeData
    {
        public IReadOnlyList<DependencyExporterFieldData> Fields { get; }
        public IReadOnlyList<DependencyExporterPropertyData> Properties { get; }

        public DependencyExporterTypeData(Type type)
        {
            var fields = new List<DependencyExporterFieldData>();
            var properties = new List<DependencyExporterPropertyData>();

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            foreach (var field in type.GetFields(flags))
            {
                if(field.FieldType.IsValueType) continue;
                var attribute = GetExportAttribute(field);
                if (attribute == null) continue;
                fields.Add(new DependencyExporterFieldData(field, attribute));
            }
            
            foreach (var property in type.GetProperties(flags))
            {
                if(property.PropertyType.IsValueType || !property.CanRead) continue;
                var attribute = GetExportAttribute(property);
                if (attribute == null) continue; 
                properties.Add(new DependencyExporterPropertyData(property, attribute));
            }

            Fields = fields;
            Properties = properties;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExportAttribute GetExportAttribute(MemberInfo member) => member.GetCustomAttribute<ExportAttribute>();

    }
}