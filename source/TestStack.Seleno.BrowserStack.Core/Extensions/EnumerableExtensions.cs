using System.Collections.Generic;
using System.Linq;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static T CyclicElementAtOrDefault<T>(this IEnumerable<T> enumerable, int index)
        {
            var arrayOfElements = enumerable as T[] ?? enumerable.ToArray();
            var numberOfElements = arrayOfElements.Length;
            return arrayOfElements.ElementAtOrDefault(index % numberOfElements);
        }
    }
}