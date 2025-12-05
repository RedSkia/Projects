using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Common
{
    public static class Exceptions
    {
        public sealed class EnumFlagsValuesException : Exception { public EnumFlagsValuesException(string? message) : base(message) { } }
        public sealed class EnumFlagsValuesOutOfBoundsException : Exception { public EnumFlagsValuesOutOfBoundsException(string? message) : base(message) { } }
        public sealed class EnumFlagsValuesMissingEmptyException : Exception { public EnumFlagsValuesMissingEmptyException(string? message) : base(message) { } }
    }
}
