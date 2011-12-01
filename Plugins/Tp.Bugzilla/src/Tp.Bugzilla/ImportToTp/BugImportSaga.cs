// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Runtime.Serialization;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Plugin.Core.Attachments;

namespace Tp.Bugzilla.ImportToTp
{
	public class BugImportSaga : TpSaga<BugImportSagaData>,
	                             IAmStartedByMessages<ImportBugToTargetProcessCommand>,
	                             IHandleMessages<BugCreatedMessage>,
	                             IHandleMessages<BugUpdatedMessage>,
	                             IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;

		public BugImportSaga()
		{
		}

		public BugImportSaga(IBugzillaInfoStorageRepository bugzillaInfoStorageRepository)
		{
			_bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<BugCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<BugUpdatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(ImportBugToTargetProcessCommand message)
		{
			Data.BugzillaBug = message.BugzillaBug;

			var tpBug = ConvertToTpBug(message);

			var storedBug = _bugzillaInfoStorageRepository.GetTargetProcessBugId(message.BugzillaBug.bug_id);

			if (storedBug == null)
			{
				Send(new CreateBugCommand(tpBug.BugDto));
				Data.CreatingBug = true;
			}
			else
			{
				tpBug.BugDto.ID = storedBug.Value;
				Send(new UpdateBugCommand(tpBug.BugDto) {ChangedFields = tpBug.ChangedFields.ToArray()});
				// We will not receive BugUpdatedMessage if no fields were changed. So we should process related entities immediately.
				SendLocal(new ExistingBugImportedToTargetProcessMessage { TpBugId = tpBug.BugDto.ID, BugzillaBug = Data.BugzillaBug });

				_bugzillaInfoStorageRepository.SaveBugzillaBugInfo(tpBug.BugDto.ID, new BugzillaBugInfo(message.BugzillaBug){TpId = tpBug.BugDto.ID});
			}
		}

		public void Handle(BugCreatedMessage message)
		{
			var tpBugId = message.Dto.ID;
			var bugzillaBug = Data.BugzillaBug;

			_bugzillaInfoStorageRepository.SaveBugsRelation(tpBugId, new BugzillaBugInfo(bugzillaBug){TpId = message.Dto.ID});

			SendLocal(new NewBugImportedToTargetProcessMessage { TpBugId = tpBugId, BugzillaBug = bugzillaBug });
			MarkAsComplete();
		}

		public void Handle(BugUpdatedMessage message)
		{
			DoNotContinueDispatchingCurrentMessageToHandlers();
			MarkAsComplete();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			if (Data.CreatingBug)
			{
				AttachmentFolder.Delete(Data.BugzillaBug.attachmentCollection.Select(x => x.FileId));
			}
			MarkAsComplete();
		}

		private ConvertedBug ConvertToTpBug(ImportBugToTargetProcessCommand message)
		{
			var converter = ObjectFactory.GetInstance<IBugConverter>();
			var tpBug = new ConvertedBug {BugDto = {ProjectID = StorageRepository().GetProfile<BugzillaProfile>().Project}};
			converter.Apply(message.BugzillaBug, tpBug);
			return tpBug;
		}
	}

	[DataContract]
	public class TargetProcessBugId
	{
		[DataMember]
		public int Value { get; set; }
	}

	[KnownType(typeof (long_desc))]
	[KnownType(typeof (attachment))]
	public class BugImportSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public BugzillaBug BugzillaBug { get; set; }
		public bool CreatingBug { get; set; }
	}
}