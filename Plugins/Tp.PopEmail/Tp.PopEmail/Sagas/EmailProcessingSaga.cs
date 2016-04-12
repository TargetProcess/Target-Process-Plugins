// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
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
	public class EmailProcessingSaga : TpSaga<EmailProcessingSagaData>,
									   IAmStartedByMessages<EmailReceivedMessage>,
	                                   IHandleMessages<MessageCreatedMessage>,
	                                   IHandleMessages<AttachmentsPushedToTPMessageInternal>,
	                                   IHandleMessages<MessageBodyUpdatedMessageInternal>,
	                                   IHandleMessages<AttachmentsAddedToGeneralMessage>,
	                                   IHandleMessages<ExceptionThrownLocalMessage>,
	                                   IHandleMessages<TargetProcessExceptionThrownMessage>,
	                                   IHandleMessages<CommentCreatedMessageInternal>,
	                                   IHandleMessages<CommentCreateFailedMessageInternal>,
									   IHandleMessages<RequestersCreationFailedMessageInternal>,
									   IHandleMessages<RequestersCreatedMessageInternal>
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
			ConfigureMapping<RequestersCreationFailedMessageInternal>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RequestersCreatedMessageInternal>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(EmailReceivedMessage emailMessage)
		{
			Data.EmailReceivedMessage = emailMessage;
			
			var requestersDtoToCreate = new List<RequesterDTO>();

			var fromAddress = new MailAddressLite { Address = emailMessage.Mail.FromAddress, DisplayName = emailMessage.Mail.FromDisplayName };
			ProcessAddress(fromAddress, requestersDtoToCreate);

			if (emailMessage.Mail.CC != null)
			{
				foreach (var ccAddress in emailMessage.Mail.CC)
				{
					ProcessAddress(ccAddress, requestersDtoToCreate);
				}
			}

			if (emailMessage.Mail.ReplyTo != null)
			{
				foreach (var replyToAddress in emailMessage.Mail.ReplyTo)
				{
					ProcessAddress(replyToAddress, requestersDtoToCreate);
				}
			}

			if (requestersDtoToCreate.Empty())
			{
				var fromRequester = GetRequesterByAddress(fromAddress.Address);
				InvokeRules(fromRequester.Id);
			}
			else
			{
				SendLocal(new CreateRequestersForMessageCommandInternal { RequestersDto = requestersDtoToCreate.ToArray(), OuterSagaId = Data.Id });
			}
		}

		private void ProcessAddress(MailAddressLite address, List<RequesterDTO> requestersDtoToCreate)
		{
			var requester = GetRequesterByAddress(address.Address);
			if (requester != null)
			{
				RestoreDeletedRequester(requester);
			}
			else
			{
				var requesterDto = CreateRequesterDtoFromAddress(address);
				requestersDtoToCreate.Add(requesterDto);
			}
		}

		private static UserLite GetRequesterByAddress(string address)
		{
			var people = ObjectFactory.GetInstance<UserRepository>().GetByEmail(address).ToArray();

			var requesters = people.Where(x => x.UserType == UserType.Requester).ToArray();
			if (requesters.Empty())
			{
				return people.Where(x => x.UserType == UserType.User).FirstOrDefault(x => !x.IsDeletedOrInactiveUser);
			}

			var requester = requesters.FirstOrDefault(x => !x.IsDeletedRequester);
			return requester ?? requesters.FirstOrDefault();
		}

		public void Handle(RequestersCreatedMessageInternal message)
		{
			var fromUsers = ObjectFactory.GetInstance<UserRepository>().GetByEmail(Data.EmailReceivedMessage.Mail.FromAddress);
			var fromUser = fromUsers.OrderBy(x => x.UserType).FirstOrDefault();
			if (fromUser == null)
			{
				Log().ErrorFormat("Failed to find user with email '{0}'. Message will not be processed", Data.EmailReceivedMessage.Mail.FromAddress);
				MarkAsComplete();
			}
			else
			{				
				InvokeRules(fromUser.Id);
			}
		}

		private IEnumerable<int> GetRequestersForEmail(EmailMessage emailMessage)
		{
			var requesterAddresses = new List<string> {emailMessage.FromAddress};
			if (emailMessage.ReplyTo != null)
			{
				requesterAddresses.AddRange(emailMessage.ReplyTo.Select(replyToAddress => replyToAddress.Address));
			}
			if (emailMessage.CC != null) 
			{
				requesterAddresses.AddRange(emailMessage.CC.Select(ccAddress => ccAddress.Address));
			}

			foreach (var requesterAddress in requesterAddresses.Distinct())
			{
				var users = ObjectFactory.GetInstance<UserRepository>().GetByEmail(requesterAddress);
				var user = users.OrderBy(x => x.UserType).FirstOrDefault();
				if (user != null && user.Id.HasValue)
				{
					yield return user.Id.Value;
				}				
			}
		}

		public void Handle(RequestersCreationFailedMessageInternal emailMessage)
		{
			Log().ErrorFormat("Failed to create message with subject '{0}'", Data.EmailReceivedMessage.Mail.Subject);
			CompleteSaga();
		}

		private static RequesterDTO CreateRequesterDtoFromAddress(MailAddressLite address)
		{
			var requesterDto = new RequesterDTO { Email = address.Address, SourceType = RequesterSourceTypeEnum.Mail };
            if (string.IsNullOrEmpty(address.DisplayName))
			{
                return requesterDto;
			}

			var name = address.DisplayName.Split(new[] { ' ' }, 2);
			switch (name.Length)
			{
				case 1:
					requesterDto.FirstName = string.IsNullOrEmpty(name[0]) ? address.Address : name[0];
					break;
				case 2:
					requesterDto.FirstName = name[0];
					requesterDto.LastName = name[1];
					break;
			}

			return requesterDto;
		}

		private void RestoreDeletedRequester(UserLite requester)
		{
			if (requester.UserType == UserType.Requester && requester.DeleteDate != null)
			{
				Send(new UpdateRequesterCommand(new RequesterDTO { ID = requester.Id, DeleteDate = null },
															new Enum[] { RequesterField.DeleteDate }));
			}
		}

		private void InvokeRules(int? fromUserId)
		{
			Data.Requesters = GetRequestersForEmail(Data.EmailReceivedMessage.Mail).ToArray();

			var messageDto = Data.EmailReceivedMessage.Mail.Convert();
			if (!MatchedRule.IsNull || MessageContainsTicket(messageDto.Body))
			{
				messageDto.FromID = fromUserId;
				Log().Info($"Creating message with subject {messageDto.Subject} in tp ");
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
			Log().Info($"Creating attachments for message with id {message.Dto.ID}");
			SendLocal(new PushAttachmentsToTpCommandInternal
			          	{
			          		OuterSagaId = Data.Id,
			          		LocalStoredAttachments = Data.EmailReceivedMessage.Mail.EmailAttachments.ToArray(),
			          		MessageId = message.Dto.ID
			          	});
		}

		public void Handle(AttachmentsPushedToTPMessageInternal message)
		{
			Log().Info($"Updating body for message with id {Data.MessageDto.ID}");
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
				Log().Info($"Creating comment from message {Data.MessageDto.ID} to general {ticketID}");
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
			Log().Info($"Adding attachments to general {message.Comment.GeneralID}");
			SendLocal(new AddAttachmentsToGeneralCommandInternal
			          	{Attachments = Data.Attachments, GeneralId = message.Comment.GeneralID, OuterSagaId = Data.Id});
		}

		public void Handle(AttachmentsAddedToGeneralMessage message)
		{
			CompleteSaga();
		}

		private void ExecuteMailRule()
		{
			var matchedRule = MatchedRule;
			Log().InfoFormat("Executing rule '{0}' on message {1}", matchedRule.ToString(), Data.MessageDto.ID);
			matchedRule.Execute(Data.MessageDto, Data.Attachments, Data.Requesters);
			CompleteSaga();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			Log().Error($"Failed to create message with subject {Data.EmailReceivedMessage.Mail.Subject}", message.GetException());
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
		public int[] Requesters { get; set; }
	}
}
