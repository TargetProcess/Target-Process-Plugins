// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus.Workflow
{
	class AutomaticSearcherListener : IHandleMessages<TickMessage>
	{
		private readonly IPluginContext _pluginContext;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		public AutomaticSearcherListener(IPluginContext pluginContext, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider)
		{
			_pluginContext = pluginContext;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
		}

		public void Handle(TickMessage message)
		{
			if (_profile.Initialized)
			{
				_documentIndexProvider.ShutdownDocumentIndexesIfRunning(_pluginContext.AccountName, new DocumentIndexShutdownSetup(forceShutdown: false, cleanStorage: false));
			}
		}
	}
}