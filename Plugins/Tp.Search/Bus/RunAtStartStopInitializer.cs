// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Commands;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus
{
	public class RunAtStartStopInitializer : IWantToRunAtStartup
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IActivityLogger _log;
		private readonly IProfileCollection _profileCollection;
		private readonly bool _isOnSite;
		private readonly ITpBus _bus;
		private readonly IPluginContext _pluginContext;
		private readonly IPluginMetadata _pluginMetadata;

		public RunAtStartStopInitializer()
		{
			_documentIndexProvider = ObjectFactory.GetInstance<IDocumentIndexProvider>();
			_log = ObjectFactory.GetInstance <IActivityLogger>();
			_profileCollection = ObjectFactory.GetInstance<IProfileCollection>();
			_isOnSite = ObjectFactory.GetInstance<IMsmqTransport>().RoutableTransportMode == RoutableTransportMode.OnSite;
			_bus = ObjectFactory.GetInstance<ITpBus>();
			_pluginContext = ObjectFactory.GetInstance<IPluginContext>();
			_pluginMetadata = ObjectFactory.GetInstance<IPluginMetadata>();
		}

		public void Run()
		{
			if (_isOnSite && _profileCollection.Empty())
			{
				var c = new BuildSearchIndexesCommand(_bus, _profileCollection, _pluginContext, _pluginMetadata);
				c.Execute(string.Empty);
			}
			_log.Info("Started Search Plugin");
		}

		public void Stop()
		{
			_log.Info("Save and free Hoot storages memory at stopping Search Plugin");
			var accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
			if (accountCollection != null && accountCollection.Any())
			{
				foreach (var account in accountCollection.Where(x => x.Profiles.Any() && !x.Profiles.First().Initialized))
				{
					_documentIndexProvider.ShutdownDocumentIndexes(account.Name, new DocumentIndexShutdownSetup(forceShutdown:true, cleanStorage:false));
				}
			}
			_log.Info("Finished to save and free Hoot storages memory at stopping Search Plugin");
		}
	}
}