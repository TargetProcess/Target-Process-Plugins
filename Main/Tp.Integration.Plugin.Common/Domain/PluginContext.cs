// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages;
using Tp.Integration.Messages.ServiceBus;

namespace Tp.Integration.Plugin.Common.Domain
{
	class PluginContext : IPluginContext
	{
		private readonly IBus _bus;
		private readonly IPluginMetadata _pluginMetadata;

		public PluginContext(IBus bus, IPluginMetadata pluginMetadata)
		{
			_bus = bus;
			_pluginMetadata = pluginMetadata;
		}

		public virtual PluginName PluginName
		{
			get { return _pluginMetadata.PluginData.Name; }
		}

		public virtual AccountName AccountName
		{
			get { return _bus.GetInAccountName(); }
		}

		public virtual ProfileName ProfileName
		{
			get { return _bus.GetInProfileName(); }
		}
	}
}