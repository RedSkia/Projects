using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Obfuscator.Common
{
    /*
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class HookableEventAttribute : Attribute { }

    public static class EventManager
    {
        private static readonly object _lock = new();
        private static readonly Dictionary<string, List<Delegate>> _events = new();
        public static IReadOnlyCollection<string> Events => _events.Keys;

        public readonly struct EventName
        {
            public readonly string RawName;
            public readonly string FullName;

            public EventName(string eventName)
            {
                if (String.IsNullOrEmpty(eventName)) 
                {
                    this.RawName = String.Empty;
                    this.FullName = String.Empty;
                    return;
                }
                var eventNameSpan = eventName.AsSpan();
                this.RawName = (eventNameSpan.StartsWith("On", StringComparison.OrdinalIgnoreCase) ? eventNameSpan.Slice(2, eventNameSpan.Length-2) : eventNameSpan).ToString();
                this.FullName = $"On{this.RawName}";
            }
            public static implicit operator bool(EventName eventName) => !(String.IsNullOrEmpty(eventName.RawName) && String.IsNullOrEmpty(eventName.FullName));
            public static implicit operator string(EventName eventName) => eventName.RawName;
        }

        private static void RemoveEmptyEvents(EventName eventName)
        {
            if(!eventName || !_events.ContainsKey(eventName) || _events[eventName]?.Count > 0) { return; }
            _events[eventName]?.Clear();
            _events.Remove(eventName);
        }
        public static bool Subscribe(params Delegate[] handles)
        {
            lock (_lock)
            {
                if(handles == null || handles.Length == 0) { return false; }

                foreach (Delegate handle in handles)
                {
                    var eventName = new EventName(handle.Method.Name);
                    if (handle == null || !eventName) { continue; }
                    if (!_events.ContainsKey(eventName)) { _events.Add(eventName, new()); }
                    _events[eventName].Add(handle);
                }
                
                return true;
            }
        }
        public static bool Unsubscribe(params Delegate[] handles)
        {
            lock (_lock)
            {
                if (handles == null || handles.Length == 0) { return false; }

                foreach (Delegate handle in handles)
                {
                    var eventName = new EventName(handle.Method.Name);
                    if (handle == null || !eventName || !_events.ContainsKey(eventName)) { continue; }
                    _events[eventName].Remove(handle);
                    RemoveEmptyEvents(eventName);
                }

                return true;
            }
        }

        public static bool? InvokeEvent(params object[] args)
        {
            lock(_lock)
            {
                var handle = GetStackTrace().GetFrame(1)?.GetMethod();
                var eventName = new EventName(handle?.Name);

                if (handle == null || !eventName) { throw new StackOverflowException("Invalid handle"); }
                if (!handle.IsDefined(typeof(HookableEventAttribute), false)) { throw new MissingFieldException($"Handle \"{handle.DeclaringType}.{eventName}\" missing HookableEventAttribute"); }
                if (!_events.TryGetValue(eventName, out var subscribers)) { return false; }

                foreach (Delegate subscriber in subscribers)
                {
                    try
                    {
                        var subscriberParameters = subscriber.Method.GetParameters().Select(p => p.ParameterType.UnderlyingSystemType);
                        var parsedParameters = args.Select(arg => arg.GetType().UnderlyingSystemType);
                        var matchingParameters = parsedParameters.TakeWhile((type, index) => index < subscriberParameters.Count() && subscriberParameters.ElementAt(index).Equals(type));
                        var resultValues = args.Take(matchingParameters.Count()).Concat(subscriberParameters.Skip(matchingParameters.Count()).Select(type => type.GetDefaultValue())).ToArray();
                        
                        subscriber?.DynamicInvoke(resultValues);
                    } 
                    catch (Exception inner)
                    { 
                        throw new ArgumentNullException($"EventManager Error at: {GetStackTrace()}", inner);
                    }
                }
                return true;
            }
        }
    }
    */
}