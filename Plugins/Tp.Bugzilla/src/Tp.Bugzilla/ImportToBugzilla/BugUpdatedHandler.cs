// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Bugzilla.ImportToBugzilla
{
	public class BugUpdatedHandler : IHandleMessages<BugUpdatedMessage>
	{
		private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;
		private readonly IBugzillaService _bugzillaService;
		private readonly EntityStateConverter _entityStateConverter;
		private readonly IActivityLogger _log;
		private readonly IBugzillaActionFactory _actionFactory;

		public BugUpdatedHandler(IBugzillaInfoStorageRepository bugzillaInfoStorageRepository, IBugzillaService bugzillaService, EntityStateConverter entityStateConverter, IActivityLogger logger, IBugzillaActionFactory actionFactory)
		{
			_bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
			_bugzillaService = bugzillaService;
			_entityStateConverter = entityStateConverter;
			_log = logger;
			_actionFactory = actionFactory;
		}

		public void Handle(BugUpdatedMessage message)
		{
			//TODO: take into account: when set the second developer in TP this developer assigned in Bugzilla 

			if (!message.ChangedFields.Contains(BugField.EntityStateID))
			{
				return;
			}

			var bugzillaBug = _bugzillaInfoStorageRepository.GetBugzillaBug(message.Dto.ID);
			if (bugzillaBug == null)
			{
				return;
			}

			var status = _entityStateConverter.GetMappedBugzillaStatus(message.Dto);
			if (status == null)
			{
				return;
			}

			try
			{
				_log.InfoFormat("Updating bug status in Bugzilla. TargetProcess Bug ID: {0}; Bugzilla Bug ID: {1}", message.Dto.ID, bugzillaBug.Id);
				_bugzillaService.Execute(_actionFactory.GetChangeStatusAction(message.Dto, bugzillaBug.Id, status));
				_log.InfoFormat("Bug status in Bugzilla updated. TargetProcess Bug ID: {0}; Bugzilla Bug ID: {1}", message.Dto.ID, bugzillaBug.Id);
			}
			catch (Exception e)
			{
				_log.Error(e.Message, e);
			}
		}
	}
}
