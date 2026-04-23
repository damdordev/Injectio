using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Damdor.Injectio
{
    public class DependencyResolverFieldData
    {
        public FieldInfo Field { get; }
        
        public DependencyResolverFieldData(FieldInfo field)
        {
            Field = field;
        }
    }

    public class DependencyResolverPropertyData
    {
        public PropertyInfo Property { get; }

        public DependencyResolverPropertyData(PropertyInfo property)
        {
            Property = property;
        }
    }

    public class DependencyResolverMethodData
    {
        public MethodInfo Method { get; }
        public Type[] Parameters { get; }

        public DependencyResolverMethodData(MethodInfo method, ParameterInfo[] parameters)
        {
            Method = method;
            
            Parameters = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                Parameters[i] = parameters[i].ParameterType;
            }
        }
    }

    public class DependencyResolverTypeData
    {
        public IReadOnlyList<DependencyResolverFieldData> Fields { get; }
        public IReadOnlyList<DependencyResolverPropertyData> Properties { get; }
        public IReadOnlyList<DependencyResolverMethodData> Methods { get; }

        public DependencyResolverTypeData(Type type)
        {
            var fields = new List<DependencyResolverFieldData>();
            var properties = new List<DependencyResolverPropertyData>();
            var methods = new List<DependencyResolverMethodData>();

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            foreach (var field in type.GetFields(flags))
            {
                if(field.FieldType.IsValueType || !HasInjectAttribute(field)) continue;
                fields.Add(new DependencyResolverFieldData(field));
            }
            
            foreach (var property in type.GetProperties(flags))
            {
                if(property.PropertyType.IsValueType || !property.CanWrite || !HasInjectAttribute(property)) continue;
                properties.Add(new DependencyResolverPropertyData(property));
            }

            foreach (var method in type.GetMethods(flags))
            {
                if (!HasInjectAttribute(method)) continue;
                var parameters = method.GetParameters();
                if(!AreAllParametersOfReferenceType(parameters)) continue;
                methods.Add(new DependencyResolverMethodData(method, parameters));
            }

            // .ToArray() reclaims internal buffer capacity left-over from using List
            Fields = fields;
            Properties = properties;
            Methods = methods;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasInjectAttribute(MemberInfo member) => member.IsDefined(typeof(InjectAttribute), false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AreAllParametersOfReferenceType(ParameterInfo[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType.IsValueType) return false;
            }

            return true;
        }
        
    }
}