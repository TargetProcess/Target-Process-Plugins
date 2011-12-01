// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Text.RegularExpressions;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Plugin.Core;
using Tp.Plugin.Core.Attachments;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Rules;

namespace Tp.PopEmailIntegration.Sagas
{
	public class EmailProcessingSaga : TpSaga<EmailProcessingSagaData>, IAmStartedByMessages<EmailReceivedMessage>,
	                                   IHandleMessages<MessageCreatedMessage>,
	                                   IHandleMessages<AttachmentsPushedToTPMessageInternal>,
	                                   IHandleMessages<MessageBodyUpdatedMessageInternal>,
	                                   IHandleMessages<AttachmentsAddedToGeneralMessage>,
	                                   IHandleMessages<ExceptionThrownLocalMessage>,
	                                   IHandleMessages<TargetProcessExceptionThrownMessage>,
	                                   IHandleMessages<CommentCreatedMessageInternal>,
	                                   IHandleMessages<CommentCreateFailedMessageInternal>,
									   IHandleMessages<RequesterCreationFailedMessageInternal>,
									   IHandleMessages<RequesterCreatedMessageInternal>

	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<MessageCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AttachmentsPushedToTPMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<MessageBodyUpdatedMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AttachmentsAddedToGeneralMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<ExceptionThrownLocalMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentCreatedMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentCreateFailedMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RequesterCreationFailedMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RequesterCreatedMessageInternal>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(EmailReceivedMessage emailMessage)
		{
			Data.EmailReceivedMessage = emailMessage;
			var userFrom = GetUserFrom(emailMessage);
			
			if (userFrom != null)
			{
				RestoreDeletedRequester(userFrom);
				InvokeRules(userFrom.Id);
			}
			else
			{
				var requesterDto = CreateRequesterDTO(emailMessage.Mail);
				SendLocal(new CreateRequesterForMessageCommandInternal { RequesterDto = requesterDto, OuterSagaId = Data.Id });
			}
		}

		private static UserLite GetUserFrom(EmailReceivedMessage emailMessage)
		{
			var people = ObjectFactory.GetInstance<UserRepository>().GetByEmail(emailMessage.Mail.FromAddress).ToArray();

			var requesters = people.Where(x => x.UserType == UserType.Requester).ToArray();
			if(!requesters.Empty())
			{
				var requesterFrom = requesters.FirstOrDefault(x => !x.IsDeletedRequester);
				return requesterFrom ?? requesters.FirstOrDefault();
			}


			return people.Where(x => x.UserType == UserType.User).FirstOrDefault(x => !x.IsDeletedOrInactiveUser);
		}

		public void Handle(RequesterCreatedMessageInternal emailMessage)
		{
			var users = ObjectFactory.GetInstance<UserRepository>().GetByEmail(Data.EmailReceivedMessage.Mail.FromAddress);
			var user = users.OrderBy(x => x.UserType).FirstOrDefault();
			if (user == null)
			{
				Log().ErrorFormat("Failed to find user with email '{0}'. Message will not be processed", Data.EmailReceivedMessage.Mail.FromAddress);
				MarkAsComplete();
			}
			else
			{
				InvokeRules(user.Id);
			}
		}

		public void Handle(RequesterCreationFailedMessageInternal emailMessage)
		{
			Log().ErrorFormat("Failed to create message with subject '{0}'", Data.EmailReceivedMessage.Mail.Subject);
			CompleteSaga();
		}

		private static RequesterDTO CreateRequesterDTO(EmailMessage messageToProcess)
		{
			var requesterDto = new RequesterDTO { Email = messageToProcess.FromAddress, SourceType = RequesterSourceTypeEnum.Mail };
            if (string.IsNullOrEmpty(messageToProcess.FromDisplayName)) 
                return requesterDto;

			var name = messageToProcess.FromDisplayName.Split(new[] { ' ' }, 2);
			switch (name.Length)
			{
				case 1:
					requesterDto.FirstName = string.IsNullOrEmpty(name[0]) ? messageToProcess.FromAddress : name[0];
					break;
				case 2:
					requesterDto.FirstName = name[0];
					requesterDto.LastName = name[1];
					break;
			}
			return requesterDto;
		}

		private void RestoreDeletedRequester(UserLite userFrom)
		{
			if (userFrom.UserType == UserType.Requester && userFrom.DeleteDate != null)
			{
				Send(new UpdateRequesterCommand(new RequesterDTO { ID = userFrom.Id, DeleteDate = null },
															new Enum[] { RequesterField.DeleteDate }));
			}
		}

		private void InvokeRules(int? fromUserId)
		{
			var messageDto = Data.EmailReceivedMessage.Mail.Convert();
			if (!MatchedRule.IsNull || MessageContainsTicket(messageDto.Body))
			{
				messageDto.FromID = fromUserId;
				Log().Info(string.Format("Creating message with subject {0} in tp ", messageDto.Subject));
				Send(new CreateMessageCommand(messageDto));
			}
			else
			{
				CompleteSaga();
			}
		}

		private IMailRule MatchedRule
		{
			get
			{
				var ruleParser = ObjectFactory.GetInstance<RuleParser>();
				return ruleParser.Parse().FirstOrDefault(x => x.IsMatched(Data.EmailReceivedMessage.Mail)) ??
				       MailRuleSafeNull.Instance;
			}
		}

		public void Handle(MessageCreatedMessage message)
		{
			Data.MessageDto = message.Dto;
			Log().Info(string.Format("Creating attachments for message with id {0}", message.Dto.ID));
			SendLocal(new PushAttachmentsToTpCommandInternal
			          	{
			          		OuterSagaId = Data.Id,
			          		LocalStoredAttachments = Data.EmailReceivedMessage.Mail.EmailAttachments.ToArray(),
			          		MessageId = message.Dto.ID
			          	});
		}

		public void Handle(AttachmentsPushedToTPMessageInternal message)
		{
			Log().Info(string.Format("Updating body for message with id {0}", Data.MessageDto.ID));
			Data.Attachments = message.AttachmentDtos;
			SendLocal(new UpdateMessageBodyCommandInternal
			          	{MessageDto = Data.MessageDto, AttachmentDtos = message.AttachmentDtos, OuterSagaId = Data.Id});
		}

		public void Handle(MessageBodyUpdatedMessageInternal message)
		{
			Data.MessageDto = message.MessageDto;

			var ticketID = FindTicketID(Data.MessageDto.Body);
			if (ticketID > 0 && !IsMessageFromTargetProcess && !IsMessageFromProject)
			{
				Log().Info(string.Format("Creating comment from message {0} to general {1}", Data.MessageDto.ID, ticketID));
				var commentDto = new CommentDTO
				                 	{
				                 		OwnerID = Data.MessageDto.FromID,
				                 		Description = Utils.FormatComment(Utils.TextToHtml(Data.MessageDto.Body ?? string.Empty)),
				                 		GeneralID = ticketID
				                 	};

				SendLocal(new CreateCommentCommandInternal {Comment = commentDto, OuterSagaId = Data.Id});
			}
			else
			{
				ExecuteMailRule();
			}
		}

		protected bool IsMessageFromProject
		{
			get
			{
				return
					StorageRepository().Get<ProjectDTO>().Any(
						x =>
						Data.EmailReceivedMessage.Mail.FromAddress.Equals(x.InboundMailReplyAddress, StringComparison.OrdinalIgnoreCase));
			}
		}

		private bool IsMessageFromTargetProcess
		{
			get
			{
				var targetProcessEmail = StorageRepository().Get<GlobalSettingDTO>().Single().SMTPSender;

				return Data.EmailReceivedMessage.Mail.FromAddress.Equals(targetProcessEmail, StringComparison.OrdinalIgnoreCase);
			}
		}

		public static bool MessageContainsTicket(string messageBody)
		{
			return FindTicketID(messageBody) > 0;
		}

		private static int FindTicketID(string messageBody)
		{
			var result = -1;
			if (string.IsNullOrEmpty(messageBody))
			{
				return result;
			}

			var pattern = new Regex("Ticket[#](\\d+)");
			var matches = pattern.Matches(messageBody);
			if (matches.Count > 0)
			{
				if (Int32.TryParse(matches[0].Groups[1].Value, out result) == false)
				{
					result = -1;
				}
			}
			return result;
		}

		public void Handle(CommentCreatedMessageInternal message)
		{
			Log().Info(string.Format("Adding attachments to general {0}", message.Comment.GeneralID));
			SendLocal(new AddAttachmentsToGeneralCommandInternal
			          	{Attachments = Data.Attachments, GeneralId = message.Comment.GeneralID, OuterSagaId = Data.Id});
		}

		public void Handle(AttachmentsAddedToGeneralMessage message)
		{
			CompleteSaga();
		}

		private void ExecuteMailRule()
		{
			MatchedRule.Execute(Data.MessageDto, Data.Attachments);
			CompleteSaga();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			Log().Error(string.Format("Failed to create message with subject {0}",
			                  Data.EmailReceivedMessage.Mail.Subject), message.GetException());
			CompleteSaga();
		}

		public void Handle(ExceptionThrownLocalMessage message)
		{
			Handle(message as TargetProcessExceptionThrownMessage);
		}

		public void Handle(CommentCreateFailedMessageInternal message)
		{
			ExecuteMailRule();
		}

		private void CompleteSaga()
		{
			AttachmentFolder.Delete(Data.EmailReceivedMessage.Mail.EmailAttachments.Select(a => a.FileId));
			MarkAsComplete();
		}
	}

	[Serializable]
	public class EmailProcessingSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public EmailReceivedMessage EmailReceivedMessage { get; set; }
		public MessageDTO MessageDto { get; set; }
		public AttachmentDTO[] Attachments { get; set; }
	}
}
