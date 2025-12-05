
using Obfuscator.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Utilities.Tools.DebugLogger;

namespace TestNameSpace;

    class Program
    {
        class Program2
        {
            public static void Main(string[] args)
            {
                object obj = null;
                ThrowIf(obj == null);

                Console.WriteLine("AYY");
            }
        }
    }


