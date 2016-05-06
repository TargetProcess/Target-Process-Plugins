using System;

namespace Tp.Core
{
	/// <summary>
	/// Current date strategy holder.
	/// </summary>
	public static class CurrentDate
	{
		/// <summary>
		/// Get the current date using the configured strategy.
		/// </summary>
		public static DateTime Value => TimeKeeper.Now;
		public static DateTime UtcValue => TimeKeeper.UtcNow;

		private static ITimeKeeper TimeKeeper { get; set; } = CurrentTimeKeeper.Instance;

		/// <summary>
		/// Use the specified delegate to get the current date.
		/// </summary>
		/// <param name="now">New delegate, or <c>null</c> to reset to the <see cref="DateTime.Now"/>.</param>
		/// <param name="utcNow">set DateTime.UtcNow</param>
		public static IDisposable Setup(Func<DateTime> now = null, Func<DateTime> utcNow = null)
		{
			if (now == null && utcNow != null)
			{
				throw new ArgumentException($"{nameof(utcNow)} should be setuped with {nameof(now)}");
			}
			var old = TimeKeeper;
			TimeKeeper = now == null ? CurrentTimeKeeper.Instance : new DefiniteTimeKeeper(now, utcNow);
			return Disposable.Create(() => TimeKeeper = old);
		}

		public static IDisposable Setup(DateTime date)
		{
			return Setup(() => date);
		}

		private interface ITimeKeeper
		{
			DateTime Now { get; }
			DateTime UtcNow { get; }
		}

		private class CurrentTimeKeeper : ITimeKeeper
		{
			private CurrentTimeKeeper()
			{
			}

			public DateTime Now => DateTime.Now;

			public DateTime UtcNow => DateTime.UtcNow;

			public static readonly ITimeKeeper Instance = new CurrentTimeKeeper();
		}

		private class DefiniteTimeKeeper : ITimeKeeper
		{
			private readonly Func<DateTime> _time;
			private readonly Func<DateTime> _utcTime;

			public DefiniteTimeKeeper(Func<DateTime> time, Func<DateTime> utcTime = null)
			{
				_time = time;
				_utcTime = utcTime ?? (() => { throw new NotImplementedException(); });
			}

			public DateTime Now => _time();
			public DateTime UtcNow => _utcTime();
		}
	}
}
