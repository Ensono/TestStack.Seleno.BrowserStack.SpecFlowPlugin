using System;
using System.Collections;
using System.Reflection;
using BoDi;
using Castle.Core.Internal;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class BoDiExtensions
    {
        public static T RegisterInstance<T>(this IObjectContainer container, T instance) where T : class
        {
            if (container == null) return instance;

            if (container.DoesNotContains(instance))
            {
                container.RegisterInstanceAs(instance, instance.GetType());
            }

            if (instance != null && container.DoesNotContains(instance.GetType().BaseType))
            {
                container.RegisterInstanceAs(instance, instance.GetType().BaseType);
            }
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