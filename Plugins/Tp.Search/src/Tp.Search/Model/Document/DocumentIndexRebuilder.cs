using System.Linq;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Commands;

namespace Tp.Search.Model.Document
{
	class DocumentIndexRebuilder
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly DocumentIndexSetup _documentIndexSetup;
		private readonly ITpBus _bus;
		private readonly IPluginContext _pluginContext;
		private readonly IPluginMetadata _pluginMetadata;
		private readonly IProfileCollection _profileCollection;
		private readonly IProfile _profile;
		private readonly IActivityLogger _logger;

		public DocumentIndexRebuilder(IDocumentIndexProvider documentIndexProvider, DocumentIndexSetup documentIndexSetup, ITpBus bus, IPluginContext pluginContext, IPluginMetadata pluginMetadata, IProfileCollection profileCollection, IProfile profile, IActivityLogger logger)
		{
			_documentIndexProvider = documentIndexProvider;
			_documentIndexSetup = documentIndexSetup;
			_bus = bus;
			_pluginContext = pluginContext;
			_pluginMetadata = pluginMetadata;
			_profileCollection = profileCollection;
			_profile = profile;
			_logger = logger;
		}

		public bool RebuildIfNeeded(bool shouldRebuildIfNoProfile = false)
		{
			if (IsRebuildInProgress())
			{
				return false;
			}
			if (shouldRebuildIfNoProfile && _profileCollection.Empty())
			{
				_logger.InfoFormat("Going to start rebuild becuase of no profile");
				DoRebuild();
				return true;
			}
			var shouldRebuildIfNoActualVersionForAnyFile = _documentIndexProvider.DocumentIndexTypes.Any(type =>
				{
					var versions = type.GetVersions(_pluginContext.AccountName, _documentIndexSetup);
					return versions.Except(new[] {type.Version}).Any();
				});
			var shouldRebuildIfNoAnyFile = _documentIndexProvider.DocumentIndexTypes.Any(type => type.GetVersions(_pluginContext.AccountName, _documentIndexSetup).Empty());
			if (shouldRebuildIfNoActualVersionForAnyFile || shouldRebuildIfNoAnyFile)
			{
				_logger.InfoFormat("Going to start rebuild becuase of {0}", shouldRebuildIfNoActualVersionForAnyFile ? "no actual version of index file" : "no actual versions of index files");
				DoRebuild();
				return true;
			}
			return false;
		}

		private bool IsRebuildInProgress()
		{
			return !_profile.IsNull && !_profile.Initialized;
		}

		private void DoRebuild()
		{
			var c = new BuildSearchIndexesCommand(_bus, _profileCollection, _profile, _pluginContext, _pluginMetadata);
			c.Execute(string.Empty);
		}
	}
}