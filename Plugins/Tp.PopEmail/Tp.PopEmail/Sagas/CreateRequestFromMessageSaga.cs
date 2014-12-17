// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Plugin.Core.Attachments;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Rules;

namespace Tp.PopEmailIntegration.Sagas
{
	internal class CreateRequestFromMessageSaga : TpSaga<CreateRequestFromMessageSagaData>,
												IAmStartedByMessages<CreateRequestFromMessageCommand>,
												IHandleMessages<RequestCreatedMessage>,
												IHandleMessages<RequestersAttachedToRequestMessageInternal>,
												IHandleMessages<MessageAttachedToGeneralMessage>,
												IHandleMessages<AttachmentsAddedToGeneralMessage>,
												IHandleMessages<MessageUpdatedMessage>,
												IHandleMessages<RequestDescriptionUpdatedMessageInternal>,
												IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<RequestCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RequestersAttachedToRequestMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<MessageAttachedToGeneralMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AttachmentsAddedToGeneralMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<MessageUpdatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RequestDescriptionUpdatedMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(CreateRequestFromMessageCommand message)
		{
			var user = Sender(message);
			if (!IsMessageFromProject(user.Email) && !IsMessageFromTargetProcess(user.Email))
			{
				Data.Attachments = message.Attachments;
				Data.Requesters = message.Requesters;
				Data.MessageDto = message.MessageDto;

				Log().Info(string.Format("Creating request from message with id {0} in project {1}", message.MessageDto.ID,
																 message.ProjectId));

				var requestDto = new RequestDTO
													{
														OwnerID = message.MessageDto.FromID,
														Name = string.IsNullOrEmpty(message.MessageDto.Subject)
																		? string.Format("Created from Message with ID {0}", message.MessageDto.ID)
																		: message.MessageDto.Subject,
														Description = Utils.TextToHtml(message.MessageDto.Body ?? string.Empty),
														ProjectID = message.ProjectId,
														SourceType = RequestSourceEnum.Email,
														IsPrivate = message.IsPrivate,
													};
				Send(new CreateRequestCommand(requestDto));
			}
			else
			{
				Log().Info(
					string.Format(
						"Not creating request from message with id {0} in project {1} because it has been sent by TargetProcess or the project itself",
						message.MessageDto.ID, message.ProjectId));
				MarkAsComplete();
			}
		}

		public void Handle(RequestCreatedMessage message)
		{
			Data.RequestId = message.Dto.ID;
			Data.RequestDto = message.Dto;
			var requestId = message.Dto.ID;
			SendLocal(new AttachRequestersToRequestCommandInternal { OuterSagaId = Data.Id, Requesters = Data.Requesters, RequestId = requestId });
		}

		public void Handle(RequestersAttachedToRequestMessageInternal message)
		{
			var messageId = Data.MessageDto.ID;
			var requestId = Data.RequestId;
			Log().Info(string.Format("Attaching message with id {0} to request with id {1}", messageId, requestId));
			Send(new AttachMessageToGeneralCommand { MessageId = messageId, GeneralId = requestId });
		}

		public void Handle(MessageAttachedToGeneralMessage message)
		{
			Log().Info(string.Format("Adding attachments to request with id {0}", message.GeneralId));
			SendLocal(new AddAttachmentsToGeneralCommandInternal { Attachments = Data.Attachments, GeneralId = message.GeneralId, OuterSagaId = Data.Id });
		}

		public void Handle(AttachmentsAddedToGeneralMessage message)
		{
			Log().Info(string.Format("Updating description for request with id {0}", Data.RequestId));
			SendLocal(new UpdateRequestDescriptionCommandInternal
				{
					RequestAttachmentDtos = message.Attachments,
					RequestDto = Data.RequestDto,
					MessageAttachmentDtos = Data.Attachments,
					OuterSagaId = Data.Id
				});
		}

		public void Handle(RequestDescriptionUpdatedMessageInternal message)
		{
			Log().Info(string.Format("Marking message with id {0} as processed", Data.MessageDto.ID));
			Data.MessageDto.IsProcessed = true;
			Send(new UpdateMessageCommand(Data.MessageDto, new Enum[] { MessageField.IsProcessed }));
		}

		public void Handle(MessageUpdatedMessage message)
		{
			Log().Info(string.Format("Request with id {0} is successfully created from message with id {1}", Data.RequestId,
															 Data.MessageDto.ID));
			MarkAsComplete();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			Log().Error("Failed to create request from message", message.GetException());
			MarkAsComplete();
		}

		private static UserLite Sender(CreateRequestFromMessageCommand message)
		{
			return ObjectFactory.GetInstance<UserRepository>().GetById(message.MessageDto.FromID);
		}

		protected bool IsMessageFromProject(string email)
		{
			return
				StorageRepository().Get<ProjectDTO>().Any(
					x => email.Equals(x.InboundMailReplyAddress, StringComparison.OrdinalIgnoreCase));
		}

		private bool IsMessageFromTargetProcess(string email)
		{
			var targetProcessEmail = StorageRepository().Get<GlobalSettingDTO>().Single().SMTPSender;

			return email.Equals(targetProcessEmail, StringComparison.OrdinalIgnoreCase);
		}
	}

	public class CreateRequestFromMessageSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public MessageDTO MessageDto { get; set; }
		public RequestDTO RequestDto { get; set; }
		public int? RequestId { get; set; }
		public AttachmentDTO[] Attachments { get; set; }
		public int[] Requesters { get; set; }
	}
}
