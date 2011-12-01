// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToTp
{
	public class BugsSynchronizer : IHandleMessages<TickMessage>
	{
		private readonly ILocalBus _bus;
		private readonly IStorageRepository _storageRepository;
		private readonly IBugzillaService _bugzillaService;
		private readonly IBugChunkSize _bugChunkSize;
		private readonly IActivityLogger _logger;

		public BugsSynchronizer(ILocalBus bus, IStorageRepository storageRepository, IBugzillaService bugzillaService,
		                        IBugChunkSize bugChunkSize, IActivityLogger logger)
		{
			_bus = bus;
			_storageRepository = storageRepository;
			_bugzillaService = bugzillaService;
			_bugChunkSize = bugChunkSize;
			_logger = logger;
		}

		public void Handle(TickMessage message)
		{
			ProcessFailedChunks();

			ProcessNewlyChanges(message.LastSyncDate);
		}

		private IStorage<FailedSyncDate> FailedSyncDates
		{
			get { return _storageRepository.Get<FailedSyncDate>(); }
		}

		private IStorage<FailedChunk> FailedChunks
		{
			get { return _storageRepository.Get<FailedChunk>(); }
		}

		private void ProcessNewlyChanges(DateTime? lastSyncDate)
		{
			var dateValue = GetSyncDate(lastSyncDate);

			int[] changedIds;

			if (TryGetChangedIds(dateValue, out changedIds))
			{
				ProcessChangedBugIds(changedIds);
			}
			else
			{
				FailedSyncDates.ReplaceWith(new FailedSyncDate(dateValue));
			}
		}

		private void ProcessChangedBugIds(int[] changedIds)
		{
			FailedSyncDates.Clear();

			var lastIndex = 0;

			var bugIdsChunk = changedIds.Skip(lastIndex).Take(_bugChunkSize.Value).ToArray();

			while (bugIdsChunk.Any())
			{
				_bus.SendLocal(new ImportBugsChunk {BugzillaBugsIds = bugIdsChunk});

				lastIndex += _bugChunkSize.Value;
				bugIdsChunk = changedIds.Skip(lastIndex).Take(_bugChunkSize.Value).ToArray();
			}
		}

		private void ProcessFailedChunks()
		{
			if (!FailedChunks.Any()) return;

			foreach (var failedChunk in FailedChunks)
			{
				_bus.SendLocal(new ImportBugsChunk {BugzillaBugsIds = failedChunk.Chunk});
			}

			FailedChunks.Clear();
		}

		private DateTime GetSyncDate(DateTime? lastSyncDate)
		{
			var lastFailedDate = FailedSyncDates.FirstOrDefault();

			DateTime dateValue;

			if (lastFailedDate != null)
			{
				dateValue = lastFailedDate.GetValue();
			}
			else
			{
				dateValue = lastSyncDate.HasValue ? lastSyncDate.Value : DateTime.MinValue;
			}
			return dateValue;
		}

		private bool TryGetChangedIds(DateTime dateValue, out int[] ids)
		{
			try
			{
				ids = _bugzillaService.GetChangedBugIds(dateValue).Distinct().ToArray();
				return true;
			}
			catch (Exception e)
			{
				_logger.ErrorFormat("Retrieving changed bug ids from Bugzilla failed for the reason : '{0}'",
				                    e.Message);
				ids = new int[] {};

				return false;
			}
		}
	}

	public class ImportBugToTargetProcessCommand : IPluginLocalMessage
	{
		public BugzillaBug BugzillaBug { get; set; }
	}

	public class ImportBugsChunk : IPluginLocalMessage
	{
		public int[] BugzillaBugsIds { get; set; }
	}
}