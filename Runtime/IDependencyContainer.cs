using System;

namespace Damdor.Injectio
{
    public interface IDependencyContainer
    {
        object Get(Type type);
    }
}