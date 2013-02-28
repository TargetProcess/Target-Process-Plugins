// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
	using System.Configuration;

	public class PluginRouterSection
		: ConfigurationSection
	{
		[ConfigurationProperty("Mode", IsRequired = true)]
		public PluginRouterMode Mode
		{
			get
			{
				return (PluginRouterMode)this["Mode"];
			}
		}

		[ConfigurationProperty("ProxyQueue", IsRequired = true)]
		public string ProxyQueue
		{
			get
			{
				return (string)this["ProxyQueue"];
			}
		}

		[ConfigurationProperty("ProxyPath", IsRequired = false, DefaultValue = "PluginRouter\\bin\\Tp.Integration.Router.exe")]
		public string ProxyPath
		{
			get
			{
				return (string)this["ProxyPath"];
			}
		}
	}
}
