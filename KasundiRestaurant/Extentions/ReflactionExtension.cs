using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KasundiRestaurant.Extentions
{
    public static class ReflactionExtension
    {
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
