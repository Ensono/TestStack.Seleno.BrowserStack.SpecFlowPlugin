using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TestStack.Seleno.BrowserStack.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumeration) where T : struct
        {
            var description = enumeration.ToString();
            var memberInfo = typeof(T).GetMember(description).FirstOrDefault();

            if (memberInfo != null)
            {
                var attribute = memberInfo.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    description = attribute.Description;
                }
            }

            return description;
        }

        public static string GetValueFor<T>(this T enumeration, string keyName, char itemSeparator = ';', char keyValueSeparator = '=') where T : struct
        {
            var description = enumeration.GetDescription();
            if (!description.Contains(itemSeparator) && !description.Contains(keyValueSeparator))
            {
                return description;
            }

            var items = enumeration.GetDescription()
                .Split(itemSeparator)
                .ToDictionary(x => x.Split(keyValueSeparator)[0], x => x.Split(keyValueSeparator)[1]);

            return items.ContainsKey(keyName) ? items[keyName] : string.Empty;
        }

        public static IEnumerable<TEnum> GetAll<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
    }
}