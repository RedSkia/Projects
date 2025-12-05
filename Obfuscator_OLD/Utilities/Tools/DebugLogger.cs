using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.Extensions;

namespace Utilities.Tools
{
    internal sealed class DebugLoggerException : global::System.Exception
    {
        private readonly string? msg;
        public DebugLoggerException(string? message = null) { this.msg = message; }
        public override string Message => "";
        public override string? StackTrace => DebugLogger.GetTrace(this.msg);
    }
    public static class DebugLogger
    {
        private const string infoTag = "[Info] >> ";
        private const string pathTag = "[Path] >> ";
        private const string fileTag = "[File] >> ";
        private static StackTrace CreateStackTrace(int skipFrames = 0, bool showFile = true) => new StackTrace(skipFrames: skipFrames, fNeedFileInfo: showFile);
        private static StackTrace GetLast(this StackTrace? stackTrace, byte framesToSubtract = 0) => CreateStackTrace(stackTrace?.FrameCount > 1 ? stackTrace.FrameCount-framesToSubtract : 0);
        private static StackTrace GetLastStackTrace() => CreateStackTrace().GetLast();
        internal static string? GetTrace(string? message = null)
        {
            StackTraceEx
            StackTrace? trace = GetLastStackTrace();


            // Get the method type
            var method = CreateStackTrace().GetLast().GetFrame(0).GetMethod() as MethodInfo;

            // Get the method's attributes
            object[] attributes = method.GetCustomAttributes(false);

            // Loop through the attributes and find the one that has the "ParameterSetName" attribute
            foreach (object attribute in attributes)
            {
                if (attribute is CustomAttributeData)
                {
                    CustomAttributeData customAttribute = (CustomAttributeData)attribute;

                    // Get the "ParameterSetName" attribute
                    var parameterSetNames = customAttribute.NamedArguments;

                    // If there is a "ParameterSetName" attribute, get the values of the parameters
                    if (parameterSetNames.Count > 0)
                    {
                        Console.WriteLine(parameterSetNames[0]);
                 
                    }
                }
            }

            return "";
            var callerMethod = trace?.GetFrame(0)?.GetMethod() as MethodInfo;
            string? scope = callerMethod?.DeclaringType?.FullName?.Replace('+', '.');
            string? returnType = callerMethod?.ReturnType?.FullName;
            string? methodName = callerMethod?.Name;
            string? methodParameters = String.Join(", ", callerMethod?.GetParameters()?.Select(parameter => parameter != null ? $"{parameter?.ParameterType} {parameter?.Name}" : null) ?? Array.Empty<string>());
            int lineNumber = trace?.GetFrame(0)?.GetFileLineNumber() ?? 0;
            string? fileName = trace?.GetFrame(0)?.GetFileName();

            var msgTrace = new StringBuilder();
            if (message != null) msgTrace.Append($"{infoTag}{message}\r\n");
            int msgLine1 = msgTrace.GetRealLength();

            msgTrace.Append($"{pathTag}");
            if (scope != null) msgTrace.Append($"{scope}");
            if (returnType != null) msgTrace.Append($" @@ {returnType}");
            if (methodName != null) msgTrace.Append($" << {methodName}");
            if (methodParameters != null) msgTrace.Append($"({methodParameters})");
            int msgLine2 = msgTrace.GetRealLength() - msgLine1;

            msgTrace.Append($"\r\n{fileTag}");
            if (fileName != null) msgTrace.Append($"{fileName} >> Line:{lineNumber}");
            else msgTrace.Append($" >> Line:{lineNumber}");
            int msgLine3 = msgTrace.GetRealLength() - msgLine1 - msgLine2;

            int msgLength = Math.Max(msgLine2, msgLine3);
            return new StringBuilder().Append('=', msgLength).Append($"\n{msgTrace}\n").Append('=', msgLength).ToString();
        }
#if DEBUG
        [StackTraceHidden]
        public static void Throw(string? message = null) => throw new DebugLoggerException(message);
#endif
        public static void ThrowIf(bool condition, string? message = null) { if (condition) Throw(message); }
        public static void ThrowIf(string? condition, string? message = null) => ThrowIf(String.IsNullOrEmpty(condition) || condition == String.Empty, message);
        public static void ThrowIf(object? condition, string? message = null)
        {
            ThrowIf(condition is null || condition == null, message); /*Reference Types*/
            ThrowIf((condition?.GetType()?.IsValueType ?? false) && condition.Equals(condition.GetType().GetDefaultValue()), message); /*Value Types*/
        }
    }
}