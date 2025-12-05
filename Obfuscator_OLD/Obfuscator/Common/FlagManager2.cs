using System;
using System.Linq;

namespace Obfuscator.Common
{
    public static class FlagManager2<T> where T : Enum
    {
        static FlagManager2()
        {
            bool HasFlagsAttribute = typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
            if(!HasFlagsAttribute) { throw new InvalidOperationException($"Enum: \"{typeof(T).Name}\" Missing FlagsAttribute"); }
        }
        public static T FirstValue => (T)Enum.ToObject(typeof(T), 0);
        /// <summary>
        /// Adds <paramref name="flags"/> to <paramref name="flag"/> as type <typeparamref name="T"/>
        /// </summary>
        /// <param name="flag">The <typeparamref name="T"/> variable</param>
        /// <param name="flags">The <paramref name="flags"/> to be added to <paramref name="flag"/> as type <typeparamref name="T"/></param>
        public static void Add(ref T flag, params T[] flags) => flag = From(To(flag) | Combine(flags));
        /// <summary>
        /// Removes the <paramref name="flags"/> from <paramref name="flag"/> as type <typeparamref name="T"/>
        /// </summary>
        /// <param name="flag">The <typeparamref name="T"/> variable</param>
        /// <param name="flags">The <paramref name="flags"/> to be removed from <paramref name="flag"/> as type <typeparamref name="T"/></param>
        public static void Remove(ref T flag, params T[] flags) => flag = From(To(flag) & ~Combine(flags));
        /// <summary>
        /// Sets the varaible <paramref name="flag"/> to the <see cref="FlagManager{T}.FirstValue"/> of <see cref="Enum"/> <typeparamref name="T"/> 
        /// </summary>
        /// <param name="flag"></param>
        public static void Clear(ref T flag) => flag = FirstValue;
        /// <summary>
        /// Checks if <paramref name="flag"/> has <paramref name="f"/>
        /// </summary>
        /// <param name="flag">The <typeparamref name="T"/> variable to be checked</param>
        /// <param name="f">The <typeparamref name="T"/> checking value for <paramref name="flag"/></param>
        /// <returns>True if <paramref name="flag"/> has value <paramref name="f"/> else false</returns>
        public static bool Has(T flag, T f) => (To(flag) & To(f)) == To(f);
        /// <summary>
        /// Checks if <paramref name="flag"/> has all values of <paramref name="flags"/>
        /// </summary>
        /// <param name="flag">The <paramref name="flag"/> variable to be checked</param>
        /// <param name="flags">The <typeparamref name="T"/> checking values for <paramref name="flag"/></param>
        /// <returns>True if <paramref name="flag"/> has all values of <paramref name="flags"/> else false</returns>
        public static bool HasAll(T flag, params T[] flags) => flags.All(f => Has(flag, f));
        /// <summary>
        /// Checks if <paramref name="flag"/> has any values of <paramref name="flags"/>
        /// </summary>
        /// <param name="flag">The <paramref name="flag"/> variable to be checked</param>
        /// <param name="flags">The <typeparamref name="T"/> checking values for <paramref name="flag"/></param>
        /// <returns>True if <paramref name="flag"/> has any values of <paramref name="flags"/> else false</returns>
        public static bool HasAny(T flag, params T[] flags) => flags.Any(f => Has(flag, f));
        /// <summary>
        /// Checks if <paramref name="flag"/> has any value of <see cref="Enum"/> <typeparamref name="T"/> without the <paramref name="exceptions"/>
        /// </summary>
        /// <param name="flag">The <paramref name="flag"/> variable to be checked</param>
        /// <param name="exceptions">The <paramref name="exceptions"/> to be excluded from the check</param>
        /// <returns>True if <paramref name="flag"/> has any values of <see cref="Enum"/> <typeparamref name="T"/> without the <paramref name="exceptions"/> else false</returns>
        public static bool HasAnyWithout(T flag, params T[] exceptions) => (To(flag) & ~Combine(exceptions)) != 0;
        /// <summary>
        /// Checks if <paramref name="flag"/> has all values of <see cref="Enum"/> <typeparamref name="T"/> without the <paramref name="exceptions"/>
        /// </summary>
        /// <param name="flag">The <paramref name="flag"/> variable to be checked</param>
        /// <param name="exceptions">The <paramref name="exceptions"/> to be excluded from the check</param>
        /// <returns>True if <paramref name="flag"/> has all values of <see cref="Enum"/> <typeparamref name="T"/> without the <paramref name="exceptions"/> else false</returns>
        public static bool HasAllWithout(T flag, params T[] exceptions) => Has(flag, From(Total(exceptions)));
        /// <summary>
        /// Converts the <see cref="Enum"/> <typeparamref name="T"/> <paramref name="value"/> to binary
        /// </summary>
        /// <param name="value">The input <see cref="Enum"/> <typeparamref name="T"/> flag</param>
        /// <returns>The binary <paramref name="value"/> of <see cref="Enum"/> <typeparamref name="T"/> in binary as <see cref="ulong"/></returns>
        public static ulong To(T value) => Convert.ToUInt64(value);
        /// <summary>
        /// Converts <paramref name="flag"/> <see cref="Char"/> to <see cref="Enum"/> <typeparamref name="T"/> value
        /// </summary>
        /// <param name="flag">The <paramref name="flag"/> <see cref="Char"/> to be converted</param>
        /// <returns><typeparamref name="T"/> flag if found else <see cref="FlagManager{T}.FirstValue"/></returns>
        public static T To(char flag) => (T)Enum.Parse(typeof(T), Enum.GetNames(typeof(T)).FirstOrDefault(name => name.ToLower().StartsWith(flag.ToString())) ?? FirstValue.ToString());
        /// <summary>
        /// Converts the binary <see cref="ulong"/> <paramref name="value"/> to <see cref="Enum"/> <typeparamref name="T"/> flag
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> binary flag <paramref name="value"/></param>
        /// <returns><see cref="Enum"/> <typeparamref name="T"/> flag <paramref name="value"/> from the binary <see cref="ulong"/> <paramref name="value"/></returns>
        public static T From(ulong value) => (T)Enum.ToObject(typeof(T), value);
        /// <summary>
        /// Combines all <see cref="Enum"/> <paramref name="flags"/> <typeparamref name="T"/> to binary
        /// </summary>
        /// <param name="flags">The <see cref="Enum"/> <paramref name="flags"/> <typeparamref name="T"/> to be combined to binary</param>
        /// <returns>The combined binary value of all <see cref="Enum"/> <paramref name="flags"/> <typeparamref name="T"/> as <see cref="ulong"/></returns>
        public static ulong Combine(params T[] flags) => flags.Aggregate(0UL, (current, flag) => current | To(flag));
        /// <summary>
        /// Combines all flags in <see cref="Enum"/> <typeparamref name="T"/> to binary without the <paramref name="exceptions"/>
        /// </summary>
        /// <param name="exceptions">The <see cref="Enum"/> <typeparamref name="T"/> <paramref name="exceptions"/> not to be combined</param>
        /// <returns>The combined binary value of all flags in <see cref="Enum"/> <typeparamref name="T"/> as <see cref="ulong"/> without the <paramref name="exceptions"/></returns>
        public static ulong Total(params T[] exceptions) => Combine(Enum.GetValues(typeof(T)).Cast<T>().Except(exceptions).ToArray());
    }
}