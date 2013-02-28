// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.BugTracking.BugFieldConverters;
using Tp.BugTracking.ImportToTp;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Plugin.Core.Attachments;

namespace Tp.Bugzilla.ImportToTp
{
	public class BugImportSaga : TpSaga<BugImportSagaData<BugzillaBug>>,
	                             IAmStartedByMessages<ImportBugToTargetProcessCommand<BugzillaBug>>,
	                             IHandleMessages<BugCreatedMessage>,
	                             IHandleMessages<BugUpdatedMessage>,
	                             IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private readonly IActivityLogger _logger;
		private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;

		public BugImportSaga()
		{
		}

		public BugImportSaga(IActivityLogger logger, IBugzillaInfoStorageRepository bugzillaInfoStorageRepository)
		{
			_logger = logger;
			_bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<BugCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<BugUpdatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(ImportBugToTargetProcessCommand<BugzillaBug> message)
		{
			Data.ThirdPartyBug = message.ThirdPartyBug;

			var tpBug = ConvertToTpBug(message);

			var storedBug = _bugzillaInfoStorageRepository.GetTargetProcessBugId(message.ThirdPartyBug.bug_id);

			if (storedBug == null)
			{
				_logger.InfoFormat("Importing bug. {0}", message.ThirdPartyBug.ToString());
				Send(new CreateBugCommand(tpBug.BugDto));
				Data.CreatingBug = true;
			}
			else
			{
				_logger.InfoFormat("Updating bug. {0}", message.ThirdPartyBug.ToString());
				tpBug.BugDto.ID = storedBug.Value;
				Send(new UpdateBugCommand(tpBug.BugDto) {ChangedFields = tpBug.ChangedFields.ToArray()});
				// We will not receive BugUpdatedMessage if no fields were changed. So we should process related entities immediately.
				SendLocal(new ExistingBugImportedToTargetProcessMessage<BugzillaBug> {TpBugId = tpBug.BugDto.ID, ThirdPartyBug = Data.ThirdPartyBug});

				_bugzillaInfoStorageRepository.SaveBugzillaBugInfo(tpBug.BugDto.ID, new BugzillaBugInfo(message.ThirdPartyBug) {TpId = tpBug.BugDto.ID});
			}
		}

		public void Handle(BugCreatedMessage message)
		{
			var tpBugId = message.Dto.ID;
			var bugzillaBug = Data.ThirdPartyBug;

			_bugzillaInfoStorageRepository.SaveBugsRelation(tpBugId, new BugzillaBugInfo(bugzillaBug) {TpId = message.Dto.ID});

			_logger.InfoFormat("Bug imported. {0}; TargetProcess Bug ID: {1}", bugzillaBug.ToString(), message.Dto.BugID);

			SendLocal(new NewBugImportedToTargetProcessMessage<BugzillaBug> {TpBugId = tpBugId, ThirdPartyBug = bugzillaBug});
			MarkAsComplete();
		}

		public void Handle(BugUpdatedMessage message)
		{
			_logger.InfoFormat("Bug updated. {0}; TargetProcess Bug ID: {1}", Data.ThirdPartyBug, message.Dto.BugID);

			DoNotContinueDispatchingCurrentMessageToHandlers();
			MarkAsComplete();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Error occured", new Exception(message.ExceptionString));

			if (Data.CreatingBug)
			{
				AttachmentFolder.Delete(Data.ThirdPartyBug.attachmentCollection.Select(x => x.FileId));
			}
			MarkAsComplete();
		}

		private ConvertedBug ConvertToTpBug(ImportBugToTargetProcessCommand<BugzillaBug> message)
		{
			var converter = ObjectFactory.GetInstance<IBugConverter<BugzillaBug>>();
			var tpBug = new ConvertedBug {BugDto = {ProjectID = StorageRepository().GetProfile<BugzillaProfile>().Project}};
			converter.Apply(message.ThirdPartyBug, tpBug);
			return tpBug;
		}
	}
}