using System;
using static Obfuscator.Common.Exceptions;
using System.Diagnostics;

namespace Obfuscator.Common
{
    /*
    public static class FlagManager
    {

        public static Enum? ToEnumInstance(this Type enumType) => enumType?.IsEnum == true ? (Activator.CreateInstance(enumType) as Enum) : null;

        #region Value Conversion
        public static ulong GetValueRaw(this Enum enumValue) => Convert.ToUInt64(enumValue);
        public static ulong GetValueRaw(this Type enumType, uint index)
        {
            Debug.

            if (enumType == null || !enumType.IsEnum) { return default; }

            var enumValues = Enum.GetValues(enumType);
            return enumValues!.Length > index ? (ulong?)enumValues!.GetValue(index) ?? 0 : 0;
        }



        public static Enum? GetValue(this string enumValue, Type enumType) => Enum.TryParse(enumType, enumValue, out object? result) ? (result as Enum) : null;
        public static Enum? GetValue(this ulong enumValue, Type enumType) => enumValue.ToString().GetValue(enumType);


        


        public static ulong GetValueCombinedRaw(this Enum[] combinedFlags)
        {
            if (combinedFlags == null || combinedFlags.Length < 1) { return default; }
            if (combinedFlags.Length >= 2 && combinedFlags.Length < 3) { return combinedFlags[0].AddRaw(combinedFlags[1]); }

            ulong combinedValue = combinedFlags[0].GetValueRaw();
            for (int i = 1; i < combinedFlags.Length; i++)
            {
                combinedValue = combinedFlags[0].AddRaw(combinedValue);
            }
            return combinedValue;
        }
        public static ulong GetValueCombinedRaw(this Enum firstValue, params Enum[] combinedFlags)
        {
            var newArray = new Enum[combinedFlags.Length + 1];
            newArray[0] = firstValue;
            Array.Copy(combinedFlags, 0, newArray, 1, combinedFlags.Length);
            return newArray.GetValueCombinedRaw();
        }
        public static Enum? GetValueCombined(this Enum[] combinedFlags) => combinedFlags.GetValueCombinedRaw().ToEnum(Enum.GetUnderlyingType(combinedFlags.GetType()));
        public static Enum? GetValueCombined(this Enum firstValue, params Enum[] combinedFlags) => firstValue.GetValueCombinedRaw(combinedFlags).ToEnum(firstValue.GetType());
        public static ulong GetFirstValueRaw(this Type enumType)
        {
            var enumValues = Enum.GetValues(enumType);
            return (enumValues?.Length > 0 ? ((ulong?)enumValues?.GetValue(0) ?? 0) : 0);
        }
        public static ulong GetFirstValueRaw(this Enum @enum) => @enum.GetType().GetFirstValueRaw();
        public static Enum? GetFirstValue(this Type enumType) => enumType.GetFirstValueRaw().ToEnum(enumType);
        public static Enum? GetFirstValue(this Enum @enum) => @enum.GetFirstValueRaw().ToEnum(@enum.GetType());
        #endregion
        #region String Conversion

        public static string ToHex(this string number, bool includePrefix = true) => (includePrefix ? "0x" : null) + Convert.ToString(number.GetValueRaw().GetUnchecked(), 16).ToUpper();
        public static string ToHex(this ulong number, bool includePrefix = true) => number.ToString().ToHex(includePrefix);
        public static string ToHex(this Enum flag, bool includePrefix = true) => flag.GetValueRaw().ToHex(includePrefix);
        public static string ToBinary(this string number, bool includePrefix = true) => (includePrefix ? "0b" : null) + Convert.ToString(number.GetValueRaw().GetUnchecked(), 2);
        public static string ToBinary(this ulong number, bool includePrefix = true) => number.ToString().ToBinary(includePrefix);
        public static string ToBinary(this Enum flag, bool includePrefix = true) => flag.GetValueRaw().ToBinary(includePrefix);
        #endregion
        #region Enum Conversion
        public static Enum? ToEnum(this string value, Type enumType) => Enum.TryParse(enumType, value, out var result) ? (result as Enum) : null;
        public static Enum? ToEnum(this ulong value, Type enumType) => value.ToString().ToEnum(enumType);
        public static ulong FromFlagToEnumRaw(this string flagName, Type enumType)
        {
            var enumNames = Enum.GetNames(enumType);
            var enumValues = Enum.GetValues(enumType);
            for (int i = 0; i < enumNames.Length; i++)
            {
                bool startsWith = enumNames[i]?.StartsWith(flagName, StringComparison.OrdinalIgnoreCase) ?? false;
                if (!startsWith) { continue; }
                return Convert.ToUInt64(enumValues.GetValue(i));
            }
            return enumType.GetFirstValueRaw();
        }
        public static Enum? FromFlagToEnum(this string flagName, Type enumType) => flagName.FromFlagToEnumRaw(enumType).ToEnum(enumType);
        #endregion



        #region Add
        public static ulong AddRaw(this ulong start, ulong value) => (start | value);
        public static ulong AddRaw(this Enum @enum, ulong value) => (@enum.GetValueRaw() | value);
        public static ulong AddRaw(this Enum @enum, string addFlagName) => @enum.AddRaw(addFlagName.FromFlagToEnumRaw(@enum.GetType()));
        public static ulong AddRaw(this Enum @enum, Enum addFlag) => @enum.AddRaw(addFlag.GetValueRaw());
        public static ulong AddRaw(this Enum @enum, params Enum[] addFlags) => addFlags.GetValueCombinedRaw();
        public static Enum? Add(this Enum @enum, ulong value) => @enum.AddRaw(value).ToEnum(@enum.GetType());
        public static Enum? Add(this Enum @enum, string addFlagName) => @enum.AddRaw(addFlagName.FromFlagToEnumRaw(@enum.GetType())).ToEnum(@enum.GetType());
        public static Enum? Add(this Enum @enum, Enum addFlag) => @enum.AddRaw(addFlag.GetValueRaw()).ToEnum(@enum.GetType());
        public static Enum? Add(this Enum @enum, params Enum[] addFlags) => @enum.AddRaw(addFlags).ToEnum(@enum.GetType());
        #endregion
        #region Remove

        #endregion
    }

    public static class FlagManager<TEnum> where TEnum : struct, Enum
    {
        static FlagManager()
        {
            CheckAttribute();
            CheckType();
            CheckLength();
            CheckValues();
        }
        #region Checks
        private static void CheckAttribute()
        {
            bool hasAttribute = typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), false)?.Length > 0;
            if (!hasAttribute)
            {
                ThrowException<InvalidOperationException>(message1: $"Enum: \"{typeof(TEnum).Name}\" is missing \"[FlagsAttribute]\"");
            }
        }
        private static void CheckType()
        {
            TypeCode enumType = GetEnumUnderlyingType;
            bool isValidType =
                enumType == TypeCode.Byte ||
                enumType == TypeCode.UInt16 ||
                enumType == TypeCode.UInt32 ||
                enumType == TypeCode.UInt64;

            if (!isValidType)
            {
                ThrowException<InvalidCastException>(message1: $"Enum \"{typeof(TEnum).Name}\" cast-type: \"{enumType}\" is invalid; Please use supported cast-types: Byte, UInt16, UInt32, UInt64");
            }
        }
        private static void CheckLength()
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            ulong maxLength = GetMaxLength;
            ulong lastHashCode = Convert.ToUInt64(enumValues?.Length > 1 ? enumValues?.GetValue((enumValues?.Length - 1 ?? 0))?.GetHashCode() : 0);
            bool exceedsLimit = lastHashCode >= maxLength;

            if (exceedsLimit)
            {
                ThrowException<OverflowException>(message1: $"Enum \"{typeof(TEnum).Name}\" exceededs limit: {maxLength}");
            }
        }
        private static void CheckValues()
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            var enumFirstValue = enumValues?.GetValue(0);

            if ((enumValues?.Length ?? 0) < 1)
            {
                ThrowException<EnumFlagsValuesOutOfBoundsException>(message1: $"Enum \"{typeof(TEnum).Name}\" have 0 members");
            }

            if (enumFirstValue == null || enumFirstValue.ToString() != "NONE" || enumFirstValue.GetHashCode() != 0x0)
            {
                ThrowException<EnumFlagsValuesMissingEmptyException>(message1: $"Enum \"{typeof(TEnum).Name}\" missing empty member named: \"NONE\" with value: \"0x0\" at the first position");
            }

            ulong nextEnumHashCode = 1;
            for (int i = 0; i < (enumValues?.Length ?? 0); i++)
            {
                var enumHashCode = Convert.ToUInt64((enumValues?.GetValue(i)?.GetHashCode()) ?? 0);

                if (i > 1)
                {
                    nextEnumHashCode <<= 1;
                    if (enumHashCode != nextEnumHashCode)
                    {
                        ThrowException<EnumFlagsValuesException>(message1: $"Enum \"{typeof(TEnum).Name}\" dosen't follow bitwise flag rules; Broken value: \"{enumHashCode}\"; Should be: \"{nextEnumHashCode}\"");
                        break;
                    }
                }
            }
        }
        private static TypeCode GetEnumUnderlyingType => Type.GetTypeCode(Enum.GetUnderlyingType(typeof(TEnum)));
        private static ulong GetMaxLength => GetEnumUnderlyingType switch
        {
            TypeCode.Byte => Byte.MaxValue,
            TypeCode.UInt16 => UInt16.MaxValue,
            TypeCode.UInt32 => UInt32.MaxValue,
            TypeCode.UInt64 => UInt64.MaxValue,
            _ => 0,
        };
        #endregion Checks
















        public static T FirstValue => (T)Enum.ToObject(typeof(T), 0);
        public static void Add(ref T flag, params T[] flags) => flag = From(To(flag) | Combine(flags));
        public static void Remove(ref T flag, params T[] flags) => flag = From(To(flag) & ~Combine(flags));
        public static void Clear(ref T flag) => flag = FirstValue;
        public static bool Has(T flag, T f) => (To(flag) & To(f)) == To(f);
        public static bool HasAll(T flag, params T[] flags) => flags.All(f => Has(flag, f));
        public static bool HasAny(T flag, params T[] flags) => flags.Any(f => Has(flag, f));
        public static bool HasAnyWithout(T flag, params T[] exceptions) => (To(flag) & ~Combine(exceptions)) != 0;
        public static bool HasAllWithout(T flag, params T[] exceptions) => Has(flag, From(Total(exceptions)));
        public static ulong To(T value) => Convert.ToUInt64(value);
        public static T To(char flag) => (T)Enum.Parse(typeof(T), Enum.GetNames(typeof(T)).FirstOrDefault(name => name.ToLower().StartsWith(flag.ToString())) ?? FirstValue.ToString());
        public static T From(ulong value) => (T)Enum.ToObject(typeof(T), value);
        public static ulong Combine(params T[] flags) => flags.Aggregate(0UL, (current, flag) => current | To(flag));
        public static ulong Total(params T[] exceptions) => Combine(Enum.GetValues(typeof(T)).Cast<T>().Except(exceptions).ToArray());

    }
    */
}