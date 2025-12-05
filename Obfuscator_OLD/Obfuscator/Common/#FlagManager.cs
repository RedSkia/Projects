using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Common
{
    /*
    public static class FlagManager3<TEnum> where TEnum : struct, Enum
    {
        static FlagManager3()
        {
            bool hasAttribute = typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
            if (!hasAttribute) { throw new InvalidOperationException($"Enum: \"{typeof(TEnum).Name}\" is missing \"[FlagsAttribute]\""); }
            
            string? firstEnumName = Enum.GetNames(typeof(TEnum)).FirstOrDefault();

            Array enumValues = Enum.GetValues(typeof(TEnum));
            dynamic firstEnumValue = enumValues.Length > 0 ?
                GetEnumUnderlyingType == TypeCode.Byte ? (byte)enumValues.GetValue(0) :
                GetEnumUnderlyingType == TypeCode.UInt16 ? (ushort)enumValues.GetValue(0) :
                GetEnumUnderlyingType == TypeCode.UInt32 ? (uint)enumValues.GetValue(0) :
                GetEnumUnderlyingType == TypeCode.UInt64 ? (ulong)enumValues.GetValue(0) : 0 : 0;

            if (String.IsNullOrEmpty(firstEnumName) || firstEnumName.ToUpper() != "NONE" || firstEnumValue != 0x0)
            {
                var frame = GetStackTrace();
                throw new MissingFieldException($"Enum: \"{typeof(TEnum).Name}\" is missing Property \"NONE\" with value \"0x0\"\n{frame}");
            }
        }

        public static BitArray Init(Enum? @enum = null)
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            CheckLength(enumValues);

            var array = new BitArray(enumValues?.Length ?? 0);
            if (@enum == null || enumValues == null) { return array; }

            for (int i = 1; i < enumValues.Length; i++)
            {
                var enumValue = enumValues.GetValue(i) as Enum;
                if (enumValue == null || !@enum.HasFlag(enumValue)) { continue; }
                array.Set(i, true);
            }
            return array;
        }
        public static TEnum ToEnum(BitArray array)
        {

            int result = 0;
            if (array == null) { return FirstValue; }
            CheckLength(array);

            for (int i = 1; i < array.Length; i++)
            {
                if (!array.Get(i)) {  continue; }
                result |= (1 << i-1);
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), result);
        }


        public static TypeCode GetEnumUnderlyingType => Type.GetTypeCode(Enum.GetUnderlyingType(typeof(TEnum)));


        public static void CheckLength(ICollection? array)
        {
            if(array == null || ((ulong)(array?.Count ?? 0) > MaxLength))
            {
                var frame = GetStackTrace();
                 throw new OverflowException($"Array \"{nameof(TEnum)}\" exceeded limit: {MaxLength}\n\r{frame}");
            }
            array = null;
        }
        public static ulong MaxLength => GetEnumUnderlyingType switch
        {
            TypeCode.Byte => Byte.MaxValue,
            TypeCode.UInt16 => UInt16.MaxValue,
            TypeCode.Int32 => Int32.MaxValue,
            TypeCode.UInt32 => UInt32.MaxValue,
            TypeCode.UInt64 => UInt64.MaxValue,
            _ => 0,
        };

        public static TEnum FirstValue => (TEnum)Enum.ToObject(typeof(TEnum), 0);


    }
    */
}
