using System;
using System.Collections.Generic;

namespace Damdor.Injectio
{
    public class DependencyContainerPack : IDependencyContainer
    {
        private readonly List<IDependencyContainer> containers = new();

        public DependencyContainerPack() {}

        public void Clear() => containers.Clear();
        
        public void Setup(IEnumerable<IDependencyContainer> containers)
        {
            this.containers.Clear();
            this.containers.AddRange(containers);
        }
        
        public DependencyContainerPack(params IDependencyContainer[] containers)
        {
            this.containers.Clear();
            this.containers.AddRange(containers);
        }

        public void Add(IDependencyContainer container) => containers.Add(container);
        public void Add(IEnumerable<IDependencyContainer> containers) => this.containers.AddRange(containers);
        public void Add(params IDependencyContainer[] containers) => this.containers.AddRange(containers);

        public object Get(Type type)
        {
            foreach (var container in containers)
            {
                var result = container.Get(type);
                if (result != null) return result;
            }

            return null;
        }
    }
}