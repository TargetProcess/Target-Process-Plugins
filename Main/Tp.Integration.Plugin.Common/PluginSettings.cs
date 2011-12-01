// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.Integration.Plugin.Common.Properties;

namespace Tp.Integration.Plugin.Common
{
	public class PluginSettings : IPluginSettings
	{
		public string PluginInputQueue
		{
			get { return Settings.Default.PluginInputQueue; }
		}
	}

	public interface IPluginSettings
	{
		string PluginInputQueue { get; }
	}
}