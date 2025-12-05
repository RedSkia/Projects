using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class DataHelper
    {
        #region String
        private static readonly char[] chars = new[] { '\'', '\"', '\\', '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' };


        public static int GetWithout(this string text)
        {
            int length = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if(Array.IndexOf(chars, text[i]) != -1) length++;
            }
            return text.Length-length;
        }
        public static int GetRealLength(this StringBuilder sb) => sb.ToString().GetRealLength();
        #endregion

        #region Ulong
        public static long ToUnchecked(this ulong value) => unchecked((long)value);
        public static ulong ToUlong(this string value) => ulong.TryParse(value, out ulong result) ? result : default;
        #endregion Ulong
    }
}
