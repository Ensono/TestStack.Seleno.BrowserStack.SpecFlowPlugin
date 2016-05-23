using System;
using System.Collections;
using BoDi;
using Castle.Core.Internal;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class BoDiExtensions
    {
        private static void RegisterInstance(this IObjectContainer container, object instance, Type type)
        {
            if (container == null || type.IsNotSubClassOrSameAs(instance)) return;

            if (container.DoesNotContains(type))
            {
                container.RegisterInstanceAs(instance, type);
            }

            container.RegisterInstance(instance, type.BaseType);
        }

        public static T RegisterInstance<T>(this IObjectContainer container, T instance) where T : class
        {
            if (container == null) return instance;

            container.RegisterInstance(instance, instance.GetType());

            return instance;
        }

        public static bool DoesNotContains(this IObjectContainer container, Type type)
        {
            return container == null || !container.Contains(type);
        }

        public static bool DoesNotContains(this IObjectContainer container, object instance)
        {
            return instance == null || container.DoesNotContains(instance.GetType());
        }

        public static bool Contains(this IObjectContainer container, Type type)
        {
            if (container == null) return false;

            var resolveAllMethod = container.GetType().GetMethod("ResolveAll").MakeGenericMethod(type);
            var results = resolveAllMethod.Invoke(container, new object[0]) as IEnumerable;

            return !results.IsNullOrEmpty();

        }
    }
}