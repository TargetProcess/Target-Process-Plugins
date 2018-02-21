using System;
using System.Diagnostics;
using System.IO;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public class Chrono
    {
        public static void TimeIt(Action target, Action<TimeSpan> handleElapsed)
        {
            TimeIt(() =>
            {
                target();

                return Unit.Default;
            }, (_, t) => handleElapsed(t));
        }

        public static T TimeIt<T>(Func<T> f, Action<TimeSpan> handleElapsed)
        {
            return TimeIt(f, (_, t) => handleElapsed(t));
        }

        public static T TimeIt<T>([InstantHandle] Func<T> f, [InstantHandle] Action<T, TimeSpan> handleElapsed)
        {
            var w = Stopwatch.StartNew();
            var result = f();
            w.Stop();
            handleElapsed(result, w.Elapsed);

            return result;
        }

        public static IDisposable TimeIt(Action<TimeSpan> handleElapsed)
        {
            var w = Stopwatch.StartNew();
            return Disposable.Create(() =>
            {
                w.Stop();
                handleElapsed(w.Elapsed);
            });
        }
    }

    public static class ChronoExtensions
    {
        public static void MeasureTime(this Action a, string name)
        {
            MeasureTime(a, name, Console.Out);
        }

        public static void MeasureTime(this Action a, string name, TextWriter into)
        {
            MeasureTime(a, elapsed => into.WriteLine("{0} elapsed {1} ms".Fmt(name, elapsed.Milliseconds)));
        }

        public static void MeasureTime(this Action a, Action<TimeSpan> handleElapsed)
        {
            Chrono.TimeIt(a, handleElapsed);
        }
    }
}
