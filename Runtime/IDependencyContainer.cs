using System;

namespace DefaultNamespace
{
    public interface IDependencyContainer
    {
        object Get(Type type);
    }
}