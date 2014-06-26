using NServiceBus;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Git.Workflow
{
	public class RepositoryRescanInitiatedMessageHandler : VersionControlSystemProcessorBase, IHandleMessages<RepositoryRescanInitiatedMessage>
	{
		public RepositoryRescanInitiatedMessageHandler(IVersionControlSystem versionControlSystem, IRevisionIdComparer revisionComparer, ILocalBus bus, ISourceControlConnectionSettingsSource settingsSource, IStorageRepository storage, IActivityLogger logger) 
			: base(revisionComparer, storage, settingsSource, versionControlSystem, bus, logger)
		{
		}

		public void Handle(RepositoryRescanInitiatedMessage message)
		{
			Logger.Info("Repository rescan started");

			ImportRevisions();
		}

		protected override RevisionRange[] RetrieveRevisionRanges()
		{
			return VersionControlSystem.GetFromTillHead(StartRevision, PageSize);
		}
	}
}