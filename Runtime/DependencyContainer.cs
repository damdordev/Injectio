using System;
using System.Collections.Generic;

namespace Damdor.Injectio
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, object> dependencies = new();

        public object Get(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            dependencies.TryGetValue(type, out var obj);
            return obj;
        }
        
        public void Register<T>(T obj) => dependencies[typeof(T)] = obj;
        public void Register(Type type, object obj) => dependencies[type] = obj;
        public void Remove(Type type) => dependencies.Remove(type);
        public void Remove<T>() => dependencies.Remove(typeof(T));
        
        public void Clear()
        {
            dependencies.Clear();
        }
    }
}