using System;
using System.Diagnostics;
using System.IO;

namespace Tp.Core
{
	public class Chrono
	{
		public static void TimeIt(Action target, Action<TimeSpan> handleElapsed)
		{
			var w = Stopwatch.StartNew();
			target();
			w.Stop();
			handleElapsed(w.Elapsed);
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

		public static void MeasureTime(this Action a, Action<TimeSpan> handleElapsedTimeInMs)
		{
			Chrono.TimeIt(a, handleElapsedTimeInMs);
		}
	}
}
