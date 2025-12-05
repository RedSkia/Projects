using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Common
{
        public readonly struct Ticks { };
        public readonly struct Micro { };
        public readonly struct Milli { };
        public readonly struct Sec { };
        public readonly struct Timer<Dur, T> : IDisposable where Dur : struct where T : IConvertible
        {
            private readonly Stopwatch _stopwatch;
            private readonly string _title;
            public Timer(string title)
            {
                _stopwatch = Stopwatch.StartNew();
                _title = title;
            }
            public readonly void Dispose()
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();

                    T result;
                    string durationString;

                    if (typeof(Dur) == typeof(Ticks))
                    {
                        result = (T)Convert.ChangeType(_stopwatch.Elapsed.Ticks, typeof(T));
                        durationString = " ticks\n";
                    }
                    else if (typeof(Dur) == typeof(Milli))
                    {
                        result = (T)Convert.ChangeType(_stopwatch.Elapsed.TotalMilliseconds, typeof(T));
                        durationString = " milliseconds\n";
                    }
                    else if (typeof(Dur) == typeof(Micro))
                    {
                        result = (T)Convert.ChangeType(_stopwatch.Elapsed.TotalMilliseconds * 1000.0, typeof(T));
                        durationString = " microseconds\n";
                    }
                    else if (typeof(Dur) == typeof(Sec))
                    {
                        result = (T)Convert.ChangeType(_stopwatch.Elapsed.TotalMilliseconds / 1000.0, typeof(T));
                        durationString = " seconds\n";
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(_stopwatch.Elapsed.Ticks, typeof(T));
                        durationString = " ticks\n";
                    }

                    Console.WriteLine($"{_title}: \t{result}\t{durationString}");
                }
            }
        }
}
