using NServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;
using Tp.Search.Model.Optimization;

namespace Tp.Search.Bus.Workflow
{
	class DocumentIndexOptimizer : IHandleMessages<TickMessage>
	{
		private readonly IPluginContext _pluginContext;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IDocumentIndexPeriodicOptimizeHint _optimizeHint;
		private readonly IActivityLogger _logger;
		public DocumentIndexOptimizer(IPluginContext pluginContext, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider, DocumentIndexSetup documentIndexSetup, DocumentIndexPeriodicOptimizeHintFactory optimizeHintFactory, IActivityLogger logger)
		{
			_pluginContext = pluginContext;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
			_optimizeHint = optimizeHintFactory.Create(documentIndexSetup);
			_logger = logger;
		}

		public void Handle(TickMessage message)
		{
			if (_profile.Initialized && _optimizeHint.Advice())
			{
				_documentIndexProvider.OptimizeDocumentIndexesIfRunning(_pluginContext, _logger);
			}
		}
	}
}