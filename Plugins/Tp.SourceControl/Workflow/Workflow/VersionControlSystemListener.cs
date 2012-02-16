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
	public class VersionControlSystemListener : IHandleMessages<TickMessage>
	{
		private readonly IVersionControlSystem _versionControlSystem;
		private readonly IRevisionIdComparer _revisionComparer;
		private readonly ILocalBus _bus;
		private readonly IStorageRepository _storage;
		private readonly IActivityLogger _logger;
		private readonly RevisionId _startRevision;

		public VersionControlSystemListener(IVersionControlSystem versionControlSystem, IRevisionIdComparer revisionComparer, ILocalBus bus,
		                                    IStorageRepository storage, ISourceControlConnectionSettingsSource settingsSource, IActivityLogger logger)
		{
			_versionControlSystem = versionControlSystem;
			_revisionComparer = revisionComparer;
			_bus = bus;
			_storage = storage;
			_logger = logger;

			_startRevision = revisionComparer.ConvertToRevisionId(settingsSource.StartRevision);
		}

		public void Handle(TickMessage message)
		{
			_logger.Info("Checking changes");

			var revisionRanges = RetrieveRevisionRanges();

			if (revisionRanges.Any())
			{
				_logger.Info("New revisions found");
			}

			revisionRanges.ForEach(x => _bus.SendLocal(new NewRevisionRangeDetectedLocalMessage {Range = x}));
			SetStartRevisionBy(revisionRanges);
		}

		private RevisionRange[] RetrieveRevisionRanges()
		{
			const int pageSize = 50;
			if (IsFirstRun)
			{
				return _versionControlSystem.GetFromTillHead(_startRevision, pageSize);
			}

			if (!IsStartRevisionChanged || IsStartRevisionWithinAlreadyProcessedRevisionRange)
			{
				var alreadyProcessedRevisionRange = _storage.Get<RevisionRange>().Single();
				return _versionControlSystem.GetAfterTillHead(alreadyProcessedRevisionRange.ToChangeset, pageSize);
			}

			if (IsStartRevisionBehindAlreadyProcessedRevisionRange)
			{
				return _versionControlSystem.GetAfterTillHead(_startRevision, pageSize);
			}

			if (IsStartRevisionBeforeAlreadyProcessedRevisionRange)
			{
				var result = new List<RevisionRange>();
				var alreadyProcessedRevisionRange = _storage.Get<RevisionRange>().Single();
				result.AddRange(_versionControlSystem.GetFromAndBefore(_startRevision, alreadyProcessedRevisionRange.FromChangeset, pageSize));
				result.AddRange(_versionControlSystem.GetAfterTillHead(alreadyProcessedRevisionRange.ToChangeset, pageSize));
				return result.ToArray();
			}

			return new RevisionRange[] {};
		}

		protected bool IsStartRevisionBeforeAlreadyProcessedRevisionRange
		{
			get { return _revisionComparer.Is(_startRevision).Before(_storage.Get<RevisionRange>().Single()); }
		}

		protected bool IsStartRevisionBehindAlreadyProcessedRevisionRange
		{
			get { return _revisionComparer.Is(_startRevision).Behind(_storage.Get<RevisionRange>().Single()); }
			}

		protected bool IsStartRevisionWithinAlreadyProcessedRevisionRange
		{
			get { return _revisionComparer.Does(_startRevision).Belong(_storage.Get<RevisionRange>().Single()); }
		}

		private bool IsStartRevisionChanged
		{
			get
			{
				var alreadyProcessedRevisionRange = _storage.Get<RevisionRange>().Single();
				return !alreadyProcessedRevisionRange.FromChangeset.Equals(_startRevision);
			}
		}

		protected bool IsFirstRun
		{
			get { return _storage.Get<RevisionRange>().Empty(); }
		}

		private void SetStartRevisionBy(RevisionRange[] revisionRanges)
		{
			if (revisionRanges.Empty())
			{
				return;
			}

			var newFromRevision = _revisionComparer.FindMinFromRevision(revisionRanges);

			var newToRevision = _revisionComparer.FindMaxToRevision(revisionRanges);

			var processedRevisionRangeStorage = _storage.Get<RevisionRange>();
			var processedRevisionRange = processedRevisionRangeStorage.SingleOrDefault() ?? new RevisionRange(newFromRevision, newToRevision);
			var newProcessedRevisionRange = new RevisionRange(GetFromRevision(processedRevisionRange, newFromRevision),
			                                                  GetToRevision(processedRevisionRange, newToRevision));

			processedRevisionRangeStorage.Clear();
			processedRevisionRangeStorage.Add(newProcessedRevisionRange);
		}

		private RevisionId GetToRevision(RevisionRange processedRevisionRange, RevisionId newToRevision)
		{
			if (_revisionComparer.Is(newToRevision).GreaterThan(processedRevisionRange.ToChangeset))
			{
				return newToRevision;
			}
			return processedRevisionRange.ToChangeset;
		}

		private RevisionId GetFromRevision(RevisionRange processedRevisionRange, RevisionId newFromRevision)
		{
			if (_revisionComparer.Is(_startRevision).LessThan(newFromRevision))
				newFromRevision = _startRevision;

			if (_revisionComparer.Is(newFromRevision).LessThan(processedRevisionRange.FromChangeset))
			{
				return newFromRevision;
			}
			return processedRevisionRange.FromChangeset;
		}
	}
}
