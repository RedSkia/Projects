using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Obfuscator.Common
{
    public static class Logger
    {
        private static readonly string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly Stack<(string, LogType)> _cachedLogs = new();
        public static IReadOnlyCollection<(string, LogType)> CachedLogs => _cachedLogs;
        private static readonly string logFilePath = Path.GetFullPath(Path.Combine(exeDir, "log.txt"));
        private static readonly object _lock = new object();

        static Logger()
        {
            lock (_lock)
            {
                using (FileStream fs = new FileStream(logFilePath, FileMode.Truncate, FileAccess.Write, FileShare.None)) { fs.SetLength(0); }
            }
        }

        //[HookableEvent]
        public static void Log(string message, LogType type, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0, [CallerFilePath] string? filePath = null)
        {
            lock (_lock)
            {
                /*
                bool hasDetailsFlag = FlagManager<LogType>.HasAny(type, LogType.Debug, LogType.Error);
                string timeStr = DateTime.Now.ToString("yyyy\"y\"/MM\"m\"/dd\"d\" - HH\"h\"\":\"mm\"m\"\":\"ss\"s\"");
                string msg = $"[{type.ToString().ToUpper()}]{((type == LogType.Warning) ? "\t" : "\t\t")}({timeStr})\tMessage: \"{message}\"{(hasDetailsFlag ? $"\tCaller: \"{member}\"\tLine: \"{line}\"\tFile: \"{filePath}\"" : "")}";
                using (StreamWriter sw = new StreamWriter(logFilePath, true)) { sw.WriteLine(msg); }
                _cachedLogs.Push((msg, type));
                InvokeEvent(msg, type);
                */
            }
        }

        [Flags]
        public enum LogType
        {
            /// <summary>
            /// Default empty value,
            /// </summary>
            NULL = 0x0,
            /// <summary>
            /// Provides in-depth information for debugging
            /// </summary>
            Debug = 0x1,
            /// <summary>
            /// Used to log messages for purposes
            /// </summary>
            Test = 0x2,
            /// <summary>
            /// Used to log information performed by the system
            /// </summary>
            Info = 0x4,
            /// <summary>
            /// Used to log non-optimal actions
            /// </summary>
            Warning = 0x8,
            /// <summary>
            /// Used to log unintended behaviour
            /// </summary>
            Error = 0x10,
        }
    }
}