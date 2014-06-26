using System.Linq;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
	public abstract class VersionControlSystemProcessorBase
	{
		protected const int PageSize = 50;

		protected VersionControlSystemProcessorBase(IRevisionIdComparer revisionComparer, IStorageRepository storage, ISourceControlConnectionSettingsSource settingsSource, IVersionControlSystem versionControlSystem, ILocalBus bus, IActivityLogger logger)
		{
			RevisionComparer = revisionComparer;
			Storage = storage;
			VersionControlSystem = versionControlSystem;
			Bus = bus;
			Logger = logger;
			StartRevision = revisionComparer.ConvertToRevisionId(settingsSource.StartRevision);
		}

		public IRevisionIdComparer RevisionComparer { get; private set; }
		public IStorageRepository Storage { get; private set; }
		public IVersionControlSystem VersionControlSystem { get; private set; }
		public ILocalBus Bus { get; private set; }
		public IActivityLogger Logger { get; private set; }
		public RevisionId StartRevision { get; private set; }

		protected void ImportRevisions()
		{
			var revisionRanges = RetrieveRevisionRanges();

			revisionRanges.ForEach(x => Bus.SendLocal(new NewRevisionRangeDetectedLocalMessage { Range = x }));

			SetStartRevisionBy(revisionRanges);
		}

		protected abstract RevisionRange[] RetrieveRevisionRanges();

		protected void SetStartRevisionBy(RevisionRange[] revisionRanges)
		{
			if (revisionRanges.Empty())
			{
				return;
			}

			var newFromRevision = RevisionComparer.FindMinFromRevision(revisionRanges);
			var newToRevision = RevisionComparer.FindMaxToRevision(revisionRanges);
			var processedRevisionRangeStorage = Storage.Get<RevisionRange>();
			var processedRevisionRange = processedRevisionRangeStorage.SingleOrDefault() ?? new RevisionRange(newFromRevision, newToRevision);
			var newProcessedRevisionRange = new RevisionRange(GetFromRevision(processedRevisionRange, newFromRevision),
			                                                  GetToRevision(processedRevisionRange, newToRevision));

			processedRevisionRangeStorage.Clear();
			processedRevisionRangeStorage.Add(newProcessedRevisionRange);
		}

		private RevisionId GetToRevision(RevisionRange processedRevisionRange, RevisionId newToRevision)
		{
			if (RevisionComparer.Is(newToRevision).GreaterThan(processedRevisionRange.ToChangeset))
			{
				return newToRevision;
			}
			return processedRevisionRange.ToChangeset;
		}

		private RevisionId GetFromRevision(RevisionRange processedRevisionRange, RevisionId newFromRevision)
		{
			if (RevisionComparer.Is(StartRevision).LessThan(newFromRevision))
				newFromRevision = StartRevision;

			if (RevisionComparer.Is(newFromRevision).LessThan(processedRevisionRange.FromChangeset))
			{
				return newFromRevision;
			}
			return processedRevisionRange.FromChangeset;
		}
	}
}