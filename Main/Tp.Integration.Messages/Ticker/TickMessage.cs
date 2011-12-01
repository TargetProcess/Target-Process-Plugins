// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Messages.Ticker
{
	/// <summary>
	/// This message is sent to synchronizable profile every specified interval.
	/// </summary>
	[Serializable]
	public class TickMessage : IPluginLocalMessage
	{
		public DateTime? LastSyncDate { get; set; }

		public TickMessage()
		{
		}

		public TickMessage(DateTime? lastSyncDate)
		{
			LastSyncDate = lastSyncDate;
		}
	}
}