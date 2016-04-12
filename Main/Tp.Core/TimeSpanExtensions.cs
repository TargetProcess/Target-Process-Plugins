namespace System
{
	public static class TimeSpanExtensions
	{
		public static double? TotalDays(this TimeSpan? timeSpan)
		{
			return timeSpan.Select(x => x.TotalDays);
		}

		public static double? TotalHours(this TimeSpan? timeSpan)
		{
			return timeSpan.Select(x => x.TotalHours);
		}

		public static double? TotalMilliseconds(this TimeSpan? timeSpan)
		{
			return timeSpan.Select(x => x.TotalMilliseconds);
		}

		public static double? TotalMinutes(this TimeSpan? timeSpan)
		{
			return timeSpan.Select(x => x.TotalMinutes);
		}

		public static double? TotalSeconds(this TimeSpan? timeSpan)
		{
			return timeSpan.Select(x => x.TotalSeconds);
		}

		public static double? TotalMonths(this TimeSpan? timeSpan)
		{
			return timeSpan.Select(x => x.TotalDays / 30);
		}
	}
}
