using System;
using NServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus.Workflow
{
	class DocumentIndexMemoryWatcher : IHandleMessages<TickMessage>
	{
		private readonly IPluginContext _pluginContext;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly DocumentIndexSetup _documentIndexSetup;
		private readonly IActivityLogger _logger;
		public DocumentIndexMemoryWatcher(IPluginContext pluginContext, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider, DocumentIndexSetup documentIndexSetup, IActivityLogger logger)
		{
			_pluginContext = pluginContext;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
			_documentIndexSetup = documentIndexSetup;
			_logger = logger;
		}

		public void Handle(TickMessage message)
		{
			if (_profile.Initialized && _documentIndexSetup.ManagedMemoryThresholdInMb != null)
			{
				long bytes = GC.GetTotalMemory(false);
				_logger.Debug("Managed memory used: {0:N} bytes".Fmt(bytes));
				if(bytes > _documentIndexSetup.ManagedMemoryThresholdInMb * 1024 * 1024)
				{
					_documentIndexProvider.OptimizeDocumentIndexesIfRunning(_pluginContext, _logger);
				}
			}
		}
	}
}