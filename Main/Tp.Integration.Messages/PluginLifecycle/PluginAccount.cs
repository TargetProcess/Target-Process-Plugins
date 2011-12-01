// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable]
	public class PluginAccount
	{
		public PluginAccount()
		{
			PluginProfiles = new PluginProfile[] {};
			Name = AccountName.Empty;
			PluginName = string.Empty;
		}

		public PluginName PluginName { get; set; }

		public AccountName Name { get; set; }

		public PluginProfile[] PluginProfiles { get; set; }
	}
}