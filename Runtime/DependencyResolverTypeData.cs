using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Damdor.Injectio
{
    internal class DependencyResolverFieldData
    {
        public FieldInfo Field { get; }
        
        public DependencyResolverFieldData(FieldInfo field)
        {
            Field = field;
        }
    }

    internal class DependencyResolverPropertyData
    {
        public PropertyInfo Property { get; }

        public DependencyResolverPropertyData(PropertyInfo property)
        {
            Property = property;
        }
    }

    internal class DependencyResolverMethodData
    {
        public MethodInfo Method { get; }
        public List<Type> Parameters { get; }

        public DependencyResolverMethodData(MethodInfo method, ParameterInfo[] parameters)
        {
            Method = method;
            Parameters = parameters.Select(parameter => parameter.ParameterType).ToList();
        }
    }

    internal class DependencyResolverTypeData
    {
        public List<DependencyResolverFieldData> Fields { get; } = new();
        public List<DependencyResolverPropertyData> Properties { get; } = new();
        public List<DependencyResolverMethodData> Methods { get; } = new();

        public DependencyResolverTypeData(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            foreach (var field in type.GetFields(flags))
            {
                if(field.FieldType.IsValueType || !HasInjectAttribute(field)) continue;
                Fields.Add(new DependencyResolverFieldData(field));
            }
            
            foreach (var property in type.GetProperties(flags))
            {
                if(property.PropertyType.IsValueType || !property.CanWrite || !HasInjectAttribute(property)) continue;
                Properties.Add(new DependencyResolverPropertyData(property));
            }

            foreach (var method in type.GetMethods(flags))
            {
                if (!HasInjectAttribute(method)) continue;
                var parameters = method.GetParameters();
                if(!AreAllParametersOfReferenceType(parameters)) continue;
                Methods.Add(new DependencyResolverMethodData(method, parameters));
            }
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