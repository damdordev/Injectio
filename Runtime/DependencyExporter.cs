using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace Damdor.Injectio
{
    public static class DependencyExporter
    {
        private static readonly Dictionary<Type, DependencyExporterTypeData> typeToData = new();
        private static readonly Dictionary<int, Stack<object[]>> parameters = new();

        public static void Export(IDependencyRegister register, object source)
        {
            if (register == null || source == null) return;

            var types = GetAllBasesTypes(source.GetType());
            try
            {
                for (var i = types.Count - 1; i >= 0; i--) Export(register, types[i], source);
            }
            finally
            {
                ListPool<Type>.Release(types);
            }
        }

        private static void Export(IDependencyRegister register, Type type, object source)
        {
            if (!typeToData.TryGetValue(type, out var data))
            {
                data = new DependencyExporterTypeData(type);
                typeToData[type] = data;
            }

            foreach (var field in data.Fields) Export(register, field, source);
            foreach (var property in data.Properties) Export(register, property, source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Export(IDependencyRegister register, DependencyExporterFieldData field, object target)
        {
            var value = field.Field.GetValue(target);
            var attribute = field.Attribute;
            if(value == null) return;
            
            if(attribute.RegisterType == null) register.Register(value);
            else register.Register(attribute.RegisterType, value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Export(IDependencyRegister register, DependencyExporterPropertyData property, object target)
        {
            var value = property.Property.GetValue(target);
            var attribute = property.Attribute;
            if(value == null) return;
            
            if(attribute.RegisterType == null) register.Register(value);
            else register.Register(attribute.RegisterType, value);
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
        
    }
}