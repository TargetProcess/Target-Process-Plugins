// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus.Workflow
{
	class AutomaticSearcherListener : IHandleMessages<TickMessage>
	{
		private readonly IPluginContext _pluginContext;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IActivityLogger _logger;
		public AutomaticSearcherListener(IPluginContext pluginContext, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider,  IActivityLogger logger)
		{
			_pluginContext = pluginContext;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
			_logger = logger;
		}

		public void Handle(TickMessage message)
		{
			if (_profile.Initialized)
			{
				_documentIndexProvider.ShutdownDocumentIndexesIfRunning(_pluginContext, new DocumentIndexShutdownSetup(forceShutdown: false, cleanStorage: false), _logger);
			}
		}
	}
}