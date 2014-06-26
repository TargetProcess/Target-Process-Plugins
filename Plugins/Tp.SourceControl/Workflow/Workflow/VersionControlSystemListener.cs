// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class VersionControlSystemListener : VersionControlSystemProcessorBase, IHandleMessages<TickMessage>
	{
		public VersionControlSystemListener(IVersionControlSystem versionControlSystem, IRevisionIdComparer revisionComparer, ILocalBus bus,
		                                    IStorageRepository storage, ISourceControlConnectionSettingsSource settingsSource, IActivityLogger logger) 
			: base(revisionComparer, storage, settingsSource, versionControlSystem, bus, logger)
		{
		}

		public void Handle(TickMessage message)
		{
			Logger.Info("Checking changes");

			ImportRevisions();
		}

		protected override RevisionRange[] RetrieveRevisionRanges()
		{
			var result = new RevisionRange[0];

			if (IsFirstRun)
			{
				result = VersionControlSystem.GetFromTillHead(StartRevision, PageSize);
			}
			else if (!IsStartRevisionChanged || IsStartRevisionWithinAlreadyProcessedRevisionRange)
			{
				var alreadyProcessedRevisionRange = Storage.Get<RevisionRange>().Single();

				result = VersionControlSystem.GetAfterTillHead(alreadyProcessedRevisionRange.ToChangeset, PageSize);
			}
			else if (IsStartRevisionBehindAlreadyProcessedRevisionRange)
			{
				result = VersionControlSystem.GetAfterTillHead(StartRevision, PageSize);
			}
			else if (IsStartRevisionBeforeAlreadyProcessedRevisionRange)
			{
				var revisionRanges = new List<RevisionRange>();
				var alreadyProcessedRevisionRange = Storage.Get<RevisionRange>().Single();

				revisionRanges.AddRange(VersionControlSystem.GetFromAndBefore(StartRevision, alreadyProcessedRevisionRange.FromChangeset, PageSize));
				revisionRanges.AddRange(VersionControlSystem.GetAfterTillHead(alreadyProcessedRevisionRange.ToChangeset, PageSize));
				
				result = revisionRanges.ToArray();
			}

			if (result.Any())
			{
				Logger.Info("New revisions found");
			}

			return result;
		}

		protected bool IsStartRevisionBeforeAlreadyProcessedRevisionRange
		{
			get { return RevisionComparer.Is(StartRevision).Before(Storage.Get<RevisionRange>().Single()); }
		}

		protected bool IsStartRevisionBehindAlreadyProcessedRevisionRange
		{
			get { return RevisionComparer.Is(StartRevision).Behind(Storage.Get<RevisionRange>().Single()); }
			}

		protected bool IsStartRevisionWithinAlreadyProcessedRevisionRange
		{
			get { return RevisionComparer.Does(StartRevision).Belong(Storage.Get<RevisionRange>().Single()); }
		}

		private bool IsStartRevisionChanged
		{
			get
			{
				var alreadyProcessedRevisionRange = Storage.Get<RevisionRange>().Single();
				return !alreadyProcessedRevisionRange.FromChangeset.Equals(StartRevision);
			}
		}

		protected bool IsFirstRun
		{
			get { return Storage.Get<RevisionRange>().Empty(); }
		}
	}
}
