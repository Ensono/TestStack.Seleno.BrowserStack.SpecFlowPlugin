using BoDi;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class ObjectContainerExtensions
    {
        public static T RegisterInstance<T>(this IObjectContainer container, T instance, string name = null)
        {
            container.RegisterInstanceAs(instance, instance.GetType(), name);
            return instance;
        }
    }
}