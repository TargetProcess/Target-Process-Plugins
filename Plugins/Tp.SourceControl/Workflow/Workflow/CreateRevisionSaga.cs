// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Messages;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class CreateRevisionSaga :
		TpSaga<CreateRevisionSagaData>,
		IAmStartedByMessages<NewRevisionDetectedLocalMessage>,
		IHandleMessages<RevisionCreatedMessage>,
		IHandleMessages<RevisionFileCreatedMessage>,
		IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private readonly IRevisionStorageRepository _revisionStorage;
		private readonly IActivityLogger _logger;
		private readonly UserMapper _userMapper;

		public CreateRevisionSaga()
		{
		}

		public CreateRevisionSaga(IRevisionStorageRepository revisionStorage, IActivityLogger logger, UserMapper userMapper) : this()
		{
			_revisionStorage = revisionStorage;
			_logger = logger;
			_userMapper = userMapper;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<NewRevisionDetectedLocalMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RevisionCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RevisionFileCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(NewRevisionDetectedLocalMessage localMessage)
		{
			string key;
			if (_revisionStorage.SaveRevisionInfo(localMessage.Revision, out key))
			{
				_logger.InfoFormat("Importing revision. Revision ID: {0}", localMessage.Revision.Id.Value);

				var dto = new RevisionDTO
				          	{
				          		SourceControlID = localMessage.Revision.Id.Value,
				          		Description = localMessage.Revision.Comment,
				          		CommitDate = localMessage.Revision.Time,
				          		AuthorID =
				          			_userMapper.IsAuthorMapped(localMessage.Revision)
				          				? (int?) _userMapper.GetAuthorBy(localMessage.Revision).Id
				          				: null
				          	};

				Data.RevisionEntries = localMessage.Revision.Entries;
				Data.SourceControlID = localMessage.Revision.Id.Value;
				Data.RevisionKey = key;

				Send(new CreateEntityCommand<RevisionDTO>(dto));
			}
			else
			{
				_logger.InfoFormat("Revision has already been imported. Aborting revision import. Revision ID: {0}", localMessage.Revision.Id.Value);
				MarkAsComplete();
			}
		}

		public void Handle(RevisionFileCreatedMessage message)
		{
			Data.RevisionFilesCreated++;

			_logger.InfoFormat(
				"Revision file created. Revision ID: {0}; TargetProcess Revision File ID: {1}; Revision File Name: {2}",
				Data.SourceControlID, message.Dto.ID.Value, message.Dto.FileName);

			if (Data.RevisionFilesCreated == Data.RevisionEntries.Length)
			{
				MarkAsComplete();
			}
		}

		public void Handle(RevisionCreatedMessage message)
		{
			Data.RevisionId = message.Dto.ID.Value;

			_revisionStorage.SaveRevisionIdTpIdRelation(message.Dto.ID.Value, message.Dto.SourceControlID);

			if (!Data.RevisionEntries.Empty())
			{
				Data.RevisionEntries.ForEach(CreateRevisionFile);
			}
			else
			{
				MarkAsComplete();
			}

			SendLocal(new RevisionCreatedLocalMessage {Dto = message.Dto});
		}

		private void CreateRevisionFile(RevisionEntryInfo revisionEntryInfo)
		{
			var dto = new RevisionFileDTO
			          	{
			          		RevisionID = Data.RevisionId,
			          		FileName = revisionEntryInfo.Path,
			          		FileAction = revisionEntryInfo.Action
			          	};

			Send(new CreateEntityCommand<RevisionFileDTO>(dto));
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_revisionStorage.RemoveRevisionInfo(Data.RevisionKey);

			_logger.Error(string.Format("Failed to create revision. Revision ID: {0}", Data.SourceControlID), message.GetException());

			MarkAsComplete();
		}
	}

	[DataContract]
	public class CreateRevisionSagaData : ISagaEntity
	{
		//this weird naming is used in order to add new property to the class and to preserve backward compatibility
		[DataMember(Name = "<Id>k__BackingField")]
		public Guid Id { get; set; }

		[DataMember(Name = "<Originator>k__BackingField")]
		public string Originator { get; set; }

		[DataMember(Name = "<OriginalMessageId>k__BackingField")]
		public string OriginalMessageId { get; set; }

		[DataMember(Name = "<RevisionEntries>k__BackingField")]
		public RevisionEntryInfo[] RevisionEntries { get; set; }

		[DataMember(Name = "<RevisionId>k__BackingField")]
		public int RevisionId { get; set; }

		[DataMember(Name = "<RevisionFilesCreated>k__BackingField")]
		public int RevisionFilesCreated { get; set; }

		[DataMember(Name = "<SourceControlID>k__BackingField")]
		public string SourceControlID { get; set; }

		[DataMember(Name = "<RevisionKey>k__BackingField")]
		public string RevisionKey { get; set; }
	}
}