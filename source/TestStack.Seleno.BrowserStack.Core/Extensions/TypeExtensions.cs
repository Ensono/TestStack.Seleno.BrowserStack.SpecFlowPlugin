using System;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSubClassOrSameAs(this Type type, object instance)
        {
            return type != null && !type.IsAbstract && type.IsClass && type.IsInstanceOfType(instance);
        }
        public static bool IsNotSubClassOrSameAs(this Type type, object instance)
        {
            return !type.IsSubClassOrSameAs(instance);
        }
    }
}