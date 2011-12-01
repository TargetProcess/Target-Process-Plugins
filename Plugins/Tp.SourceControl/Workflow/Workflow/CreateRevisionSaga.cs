// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.SourceControl.Messages;
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
		private readonly UserMapper _userMapper;

		public CreateRevisionSaga()
		{
			_userMapper = new UserMapper(StorageRepository, Log);
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
			// TODO: any other place?
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			var dto = new RevisionDTO
			{
				SourceControlID = long.Parse(localMessage.Revision.Id.Value),
				Description = localMessage.Revision.Comment,
				CommitDate = localMessage.Revision.Time,
				AuthorID =
					_userMapper.IsAuthorMapped(localMessage.Revision)
						? (int?) _userMapper.GetAuthorBy(localMessage.Revision).Id
						: null
			};

			Data.RevisionEntries = localMessage.Revision.Entries;

			Send(new CreateEntityCommand<RevisionDTO>(dto));
		}

		public void Handle(RevisionFileCreatedMessage message)
		{
			Data.RevisionFilesCreated++;

			if (Data.RevisionFilesCreated == Data.RevisionEntries.Length)
			{
				MarkAsComplete();
			}
		}

		public void Handle(RevisionCreatedMessage message)
		{
			Data.RevisionId = message.Dto.ID.Value;

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
			Log().Error("Failed to create revision", message.GetException());
			MarkAsComplete();
		}
	}

	[Serializable]
	public class CreateRevisionSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public RevisionEntryInfo[] RevisionEntries { get; set; }

		public int RevisionId { get; set; }
		public int RevisionFilesCreated { get; set; }
	}
}