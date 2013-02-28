// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Mercurial.VersionControlSystem
{
	internal static class DateTimeExtensions
	{
		private static readonly long EPOCH_TICKS;

		static DateTimeExtensions()
		{
			var time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			EPOCH_TICKS = time.Ticks;
		}

		public static long GetTime(this DateTime dateTime)
		{
			return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
		}

		public static long ToMillisecondsSinceEpoch(this DateTime dateTime)
		{
			if (dateTime.Kind != DateTimeKind.Utc)
			{
				throw new ArgumentException("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
			}
			return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
		}

		public static long ToMillisecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
		{
			return (((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) - EPOCH_TICKS)/TimeSpan.TicksPerMillisecond);
		}
	}
}