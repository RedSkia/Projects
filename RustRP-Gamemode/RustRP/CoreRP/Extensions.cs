using System;
using System.Linq;

namespace CoreRP
{
    public static class Extensions
    {
        public static string ElementAt<T>(this T[] value, int index = 0) => (value != null && value.Length > index) ? (value[index] as String) : String.Empty;
        public static bool IsType<T>(this T obj, params Type[] types) where T : class => types != null && types.Length > 0 && types.Contains(obj.GetType());
    }
}