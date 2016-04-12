// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Core;

namespace Tp.Integration.Plugin.Common.Activity
{
	/// <summary>
	/// Provides access to current profile log.
	/// </summary>
	public interface IActivityLog
	{
		ActivityDto GetBy(ActivityFilter filter);

		void Remove();

		void ClearBy(ActivityFilter filter);

		bool CheckForErrors();
	}

	internal class ActivityLogSafeNull : SafeNull<ActivityLogSafeNull, IActivityLog>, IActivityLog, INullable
	{
		private readonly ActivityDto _activityDto = new ActivityDto();

		public ActivityDto GetBy(ActivityFilter filter)
		{
			return _activityDto;
		}

		public void Remove()
		{
		}

		public void ClearBy(ActivityFilter filter)
		{
		}

		public bool CheckForErrors()
		{
			return false;
		}
	}
}