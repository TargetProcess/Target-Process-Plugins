// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using Mono.Unix.Native;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	[Serializable]
	public class GitRevisionId : IComparable
	{
		public static readonly DateTime UtcTimeMin = NativeConvert.ToDateTime(0);
		public static readonly DateTime UtcTimeMax = new DateTime(2038, 01, 19);
		public static readonly GitRevisionId MinValue = UtcTimeMin;
		public static readonly GitRevisionId MaxValue = UtcTimeMax;

		public GitRevisionId() {}

		public GitRevisionId(RevisionId revision)
			: this(ConvertToRevision(revision)) {}

		private static DateTime ConvertToRevision(RevisionId revision)
		{
			long seconds;

			if (long.TryParse(revision.Value, out seconds))
			{
				return UtcTimeMin.AddSeconds(seconds);
			}

			DateTime dateTime;
			if (DateTime.TryParse(revision.Value, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out dateTime))
			{
				return dateTime;
			}
			
			return UtcTimeMin;
		}

		public GitRevisionId(DateTime value)
		{
			Value = value < UtcTimeMin ? UtcTimeMin : value;
		}

		public DateTime Value { get; set; }

		public static implicit operator GitRevisionId(RevisionId revisionId)
		{
			return new GitRevisionId(revisionId);
		}

		public static implicit operator GitRevisionId(DateTime revisionId)
		{
			return new GitRevisionId(revisionId);
		}

		public static implicit operator GitRevisionId(long commitTime)
		{
			return new GitRevisionId(new RevisionId {Value = commitTime.ToString()});
		}

		public static implicit operator RevisionId(GitRevisionId revisionId)
		{
			return new RevisionId {Value = ConvertToUnixTimestamp(revisionId.Value).ToString()};
		}

		public int CompareTo(object obj)
		{
			if (obj is RevisionId || obj is GitRevisionId)
			{
				var thatRevisionId = (GitRevisionId) obj;
				return Value.CompareTo(thatRevisionId.Value);
			}
			return 0;
		}

		private static double ConvertToUnixTimestamp(DateTime date)
		{
			return (date - UtcTimeMin).TotalSeconds;
		}

		public override string ToString()
		{
			return string.Format("RevisionId: {0}, Date: {1}", Value.GetTime(),  Value) ;
		}
	}

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