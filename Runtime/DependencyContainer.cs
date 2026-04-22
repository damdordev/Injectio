using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, object> dependencies = new();

        public object Get(Type type)
        {
            dependencies.TryGetValue(type, out var obj);
            return obj;
        }
        
        public void Register<T>(T obj) => dependencies[typeof(T)] = obj;
        public void Register(Type type, object obj) => dependencies[type] = obj;
    }
}