using System;

namespace Tp.BugTracking.ImportToTp
{
	[Serializable]
	public class LastSyncDate
	{
		public LastSyncDate()
		{
		}

		public LastSyncDate(DateTime? failedSyncDate)
		{
			_value = failedSyncDate;
		}

		private readonly DateTime? _value;

		public DateTime? GetValue()
		{
			return _value;
		}
	}
}