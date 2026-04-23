using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace Damdor.Injectio
{
    /// <summary>
    /// A static utility class responsible for resolving and injecting dependencies into target objects.
    /// It caches reflection data to minimize allocations and uses pooling for method parameters.
    /// </summary>
    public static class DependencyResolver
    {
        private static readonly Dictionary<Type, DependencyResolverTypeData> typeToData = new();
        private static readonly Dictionary<int, Stack<object[]>> parameters = new();

        /// <summary>
        /// Resolves and injects dependencies into the specified target object using the provided dependency container.
        /// </summary>
        /// <param name="container">The dependency container used to resolve required types.</param>
        /// <param name="target">The target object to inject dependencies into.</param>
        public static void Resolve(IDependencyContainer container, object target)
        {
            if (container == null || target == null) return;

            var types = GetAllBasesTypes(target.GetType());
            try
            {
                for (var i = types.Count - 1; i >= 0; i--) Resolve(container, types[i], target);
            }
            finally
            {
                ListPool<Type>.Release(types);
            }
        }

        private static void Resolve(IDependencyContainer container, Type type, object target)
        {
            if (!typeToData.TryGetValue(type, out var data))
            {
                data = new DependencyResolverTypeData(type);
                typeToData[type] = data;
            }

            foreach (var field in data.Fields) Resolve(container, field, target);
            foreach (var property in data.Properties) Resolve(container, property, target);
            foreach (var method in data.Methods) Resolve(container, method, target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Resolve(IDependencyContainer container, DependencyResolverFieldData field, object target)
        {
            field.Field.SetValue(target, container.Get(field.Field.FieldType));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Resolve(IDependencyContainer container, DependencyResolverPropertyData property, object target)
        {
            property.Property.SetValue(target, container.Get(property.Property.PropertyType));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Resolve(IDependencyContainer container, DependencyResolverMethodData method, object target)
        {
            var parameterArray = GetParameterArray(method.Parameters.Length);

            try
            {
                FillParameterArray(container, method.Parameters, parameterArray);
                method.Method.Invoke(target, parameterArray);
            }
            finally
            {
                ReleaseParameterArray(parameterArray);   
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Type> GetAllBasesTypes(Type type)
        {
            var types = ListPool<Type>.Get();
            while (type != null && type != typeof(object))
            {
                types.Add(type);
                type = type.BaseType;
            }

            return types;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object[] GetParameterArray(int parameterCount)
        {
            if (parameters.TryGetValue(parameterCount, out var p))
            {
                return p.Count > 0 ? p.Pop() : new object[parameterCount];
            }

            p = new Stack<object[]>();
            parameters[parameterCount] = p;

            return p.Count > 0 ? p.Pop() : new object[parameterCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillParameterArray(IDependencyContainer container, Type[] parameters, object[] parameterArray)
        {
            for (var i = 0; i < parameterArray.Length; i++)
            {
                parameterArray[i] = container.Get(parameters[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReleaseParameterArray(object[] parameterArray)
        {
            for (var i = 0; i < parameterArray.Length; i++)
            {
                parameterArray[i] = null;
            }
            
            if (!parameters.TryGetValue(parameterArray.Length, out var p))
            {
                p = new Stack<object[]>();
                parameters[parameterArray.Length] = p;
            }
            
            p.Push(parameterArray);
        }
        
    }
}