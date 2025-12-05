using System;
using System.Linq;

namespace CoreRP
{
    internal static class FlagManager<T>
    {
        static FlagManager()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type T must be an enumeration type.");
            }
        }
        internal static T Default => (T)Enum.ToObject(typeof(T), 0);
        internal static void Add(ref T flag, params T[] flags) => flag = From(To(flag) | Combine(flags));
        internal static void Remove(ref T flag, params T[] flags) => flag = From(To(flag) & ~Combine(flags));
        internal static void Clear(ref T flag) => flag = Default;
        internal static bool Has(T flag, T f) => (To(flag) & To(f)) == To(f);
        internal static bool Has(T flag, params T[] flags) => flags.All(f => Has(flag, f));
        internal static bool HasAny(T flag, params T[] exceptions) => (To(flag) & ~Combine(exceptions)) != 0;
        internal static bool HasAll(T flag, params T[] exceptions) => Has(flag, From(Total(exceptions)));
        internal static ulong To(T value) => Convert.ToUInt64(value);
        internal static T To(char flag) => (T)Enum.Parse(typeof(T), Enum.GetNames(typeof(T)).FirstOrDefault(name => name.ToLower().StartsWith(flag.ToString())) ?? Default.ToString());
        internal static T From(ulong value) => (T)Enum.ToObject(typeof(T), value);
        internal static ulong Combine(params T[] flags) => flags.Aggregate(0UL, (current, flag) => current | To(flag));
        internal static ulong Total(params T[] exceptions) => Combine(Enum.GetValues(typeof(T)).Cast<T>().Except(exceptions).ToArray());
    }
}