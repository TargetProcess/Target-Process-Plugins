// 
//   Copyright (c) 2005-2011 TargetProcess. All rights reserved.
//   TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Messages.AccountLifecycle
{
	using System;
	using PluginLifecycle;

	[Serializable]
	public sealed class AccountRemovedLastStepMessage
		: IPluginLocalMessage
	{
		public AccountRemovedLastStepMessage()
		{
		}

		public AccountRemovedLastStepMessage(string accountName)
		{
			AccountName = accountName;
		}

		public string AccountName { get; set; }
	}
}
