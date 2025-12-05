using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class Reflection
    {
        public static object? GetDefaultValue(this Type type) => Type.GetTypeCode(type) switch
        {
            TypeCode.Empty => null,
            TypeCode.Object => default(Object),
            TypeCode.DBNull => default(DBNull),
            TypeCode.Boolean => default(Boolean),
            TypeCode.Char => default(Char),
            TypeCode.SByte => default(SByte),
            TypeCode.Byte => default(Byte),
            TypeCode.Int16 => default(Int16),
            TypeCode.UInt16 => default(UInt16),
            TypeCode.Int32 => default(Int32),
            TypeCode.UInt32 => default(UInt32),
            TypeCode.Int64 => default(Int64),
            TypeCode.UInt64 => default(UInt64),
            TypeCode.Single => default(Single),
            TypeCode.Double => default(Double),
            TypeCode.Decimal => default(Decimal),
            TypeCode.DateTime => default(DateTime),
            TypeCode.String => default(String),
            _ => (Nullable.GetUnderlyingType(type) != null) ? null :
                (type.IsEnum && Enum.GetValues(type).Length > 0) ? Enum.GetValues(type).GetValue(0) :
                Activator.CreateInstance(type)
        };
    }
}
