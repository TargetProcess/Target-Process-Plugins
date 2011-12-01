// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Integration.Plugin.Common.Activity
{
	public class ActivityLogRecord
	{
		public DateTime DateTime { get; set; }

		public string Level { get; set; }

		public string Message { get; set; }

		public string Details { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as ActivityLogRecord);
		}

		public bool Equals(ActivityLogRecord other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.DateTime.Equals(DateTime) && Equals(other.Level, Level) && Equals(other.Message, Message) && Equals(other.Details, Details);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = DateTime.GetHashCode();
				result = (result*397) ^ (Level != null ? Level.GetHashCode() : 0);
				result = (result*397) ^ (Message != null ? Message.GetHashCode() : 0);
				result = (result * 397) ^ (Details != null ? Details.GetHashCode() : 0);
				return result;
			}
		}
	}
}