// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Core;

namespace Tp.Integration.Messages.Ticker
{
	[Serializable]
	public class LastSyncDate
	{
		public LastSyncDate()
		{
			Value = CurrentDate.Value;
		}

		public LastSyncDate(DateTime lastSyncDate)
		{
			Value = lastSyncDate;
		}

		public DateTime Value { get; set; }
	}
}