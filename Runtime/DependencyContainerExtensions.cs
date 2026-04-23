namespace Damdor.Injectio
{
    public static class DependencyContainerExtensions
    {
        public static T Get<T>(this IDependencyContainer container) where T : class
        {
            return (T)container.Get(typeof(T));
        }
    }
}