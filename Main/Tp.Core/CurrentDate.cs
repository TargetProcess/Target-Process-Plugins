using System;

namespace Tp.Core
{
	/// <summary>
	/// Current date strategy holder.
	/// </summary>
	public static class CurrentDate
	{
		private const string TIME_KEEPER_NAME = "TimeKeeper";

		/// <summary>
		/// Get the current date using the configured strategy.
		/// </summary>
		public static DateTime Value
		{
			get
			{
				var timeKeeper = TimeKeeper;
				return timeKeeper.Now;
			}
		}

		static private ITimeKeeper _timeKeeper = CurrentTimeKeeper.Instance;

		private static ITimeKeeper TimeKeeper
		{
			get { return _timeKeeper; }
			set { _timeKeeper = value; }
		}

		/// <summary>
		/// Use the specified delegate to get the current date.
		/// </summary>
		/// <param name="getter">New delegate, or <c>null</c> to reset to the <see cref="DateTime.Now"/>.</param>
		public static IDisposable Setup(Func<DateTime> getter = null)
		{
			var oldKeeper = TimeKeeper;
			TimeKeeper = getter == null ? CurrentTimeKeeper.Instance : new DefiniteTimeKeeper(getter);
			return Disposable.Create(() => TimeKeeper = oldKeeper);
		}

		public static IDisposable Setup(DateTime date)
		{
			return Setup(() => date);
		}

		private interface ITimeKeeper
		{
			DateTime Now { get; }
		}

		private class CurrentTimeKeeper : ITimeKeeper
		{
			private CurrentTimeKeeper()
			{
			}

			public DateTime Now
			{
				get { return DateTime.Now; }
			}

			public static readonly ITimeKeeper Instance = new CurrentTimeKeeper();
		}

		private class DefiniteTimeKeeper : ITimeKeeper
		{
			private readonly Func<DateTime> _time;

			public DefiniteTimeKeeper(Func<DateTime> time)
			{
				_time = time;
			}

			public DateTime Now
			{
				get { return _time(); }
			}
		}
	}
}
