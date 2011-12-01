// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable]
	public class PluginInfo
	{
		public PluginInfo()
			: this(string.Empty)
		{
		}

		public PluginInfo(PluginName name)
		{
			Name = name;
			Accounts = new PluginAccount[] {};
		}

		public PluginName Name { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
		public PluginAccount[] Accounts { get; set; }
		public string PluginInputQueue { get; set; }
		public string PluginIconContent { get; set; }
	}
}