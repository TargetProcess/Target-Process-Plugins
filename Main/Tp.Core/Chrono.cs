using System;
using System.Diagnostics;
using System.IO;

namespace Tp.Core
{
	public class Chrono
	{
		public static void TimeIt(Action a, Action<TimeSpan> handleElapsedTimeInMs)
		{
			var w = Stopwatch.StartNew();
			a();
			w.Stop();
			handleElapsedTimeInMs(w.Elapsed);
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
