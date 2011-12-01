// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature
{
	public class EmailProcessingSagaContext : PopEmailIntegrationContext
	{
		private readonly List<MessageDTO> _createdMessageDtos;
		private CommentDTO _createdCommentDto;
		private readonly EmailMessage _emailMessage;
		private readonly Guid _sagaId = Guid.NewGuid();
		private readonly IList<AttachmentDTO> _attachments = new List<AttachmentDTO>();
		private string _createdRequesterEmail;

		public const int CREATED_MESSAGE_DTO_ID = 10;
		private const int CREATED_REQUESTER_DTO_ID = 11;

		public EmailProcessingSagaContext()
		{
			ObjectFactory.Configure(x => x.For<EmailProcessingSagaContext>().Use(this));

			_emailMessage = new EmailMessage
			{
				FromDisplayName = "John Smith",
				SendDate = DateTime.Now.AddDays(-1),
				MessageUidDto = new MessageUidDTO {UID = new Random().Next().ToString()}
			};

			_createdMessageDtos = new List<MessageDTO>();

			Transport.On<CreateCommand>(x => x.Dto is RequesterDTO)
				.Reply(x =>
				       {
				       	var emailToCreate = ((RequesterDTO) x.Dto).Email;
				       	if (_createdRequesterEmail == emailToCreate)
				       	{
				       		return new EmptyMessage();
				       	}

				       	_createdRequesterEmail = emailToCreate;
				       	return new RequesterCreatedMessage
				       	{
				       		Dto = new RequesterDTO
				       		{
				       			Email = ((RequesterDTO) x.Dto).Email,
				       			ID = CREATED_REQUESTER_DTO_ID
				       		}
				       	};
				       });

			Transport.On<CreateMessageCommand>()
				.Reply(x =>
				       {
				       	var message = new MessageDTO
				       	{
				       		Subject = x.Dto.Subject,
				       		FromID = x.Dto.FromID,
				       		Body = x.Dto.Body,
				       		ID = CREATED_MESSAGE_DTO_ID
				       	};
				       	_createdMessageDtos.Add(message);
				       	return new MessageCreatedMessage {Dto = message};
				       });

			Transport.On<CreateCommand>(x => x.Dto is CommentDTO).Reply(x => GetCommentCreatedMessage(x));

			Transport.On<CreateCommand>(x => x.Dto is AttachmentDTO)
				.Reply(message => new AttachmentCreatedMessage
				{
					Dto =
						new AttachmentDTO {ID = 11, OriginalFileName = ((AttachmentDTO) message.Dto).OriginalFileName}
				});

			GetCommentCreatedMessage = GetCommentCreateCommandSuccessfulMessage;
		}

		private Func<CreateCommand, ISagaMessage> GetCommentCreatedMessage;

		private ISagaMessage GetCommentCreateCommandSuccessfulMessage(CreateCommand x)
		{
			var commandDto = (CommentDTO) x.Dto;
			_createdCommentDto = new CommentDTO
			{
				OwnerID = commandDto.OwnerID,
				GeneralID = commandDto.GeneralID,
				Description = commandDto.Description
			};
			return new CommentCreatedMessage {Dto = _createdCommentDto};
		}

		private static ISagaMessage GetCommentCreateCommandFailedMessage(CreateCommand x)
		{
			return new TargetProcessExceptionThrownMessage();
		}

		public bool CommentCreateIsAlwaysFailed
		{
			set
			{
				if (value)
				{
					GetCommentCreatedMessage = GetCommentCreateCommandFailedMessage;
				}
				else
				{
					GetCommentCreatedMessage = GetCommentCreateCommandSuccessfulMessage;
				}
			}
		}

		public EmailMessage EmailMessage
		{
			get { return _emailMessage; }
		}

		public IList<AttachmentDTO> Attachments
		{
			get { return _attachments; }
		}

		public Guid SagaId
		{
			get { return _sagaId; }
		}

		public MessageDTO[] CreatedMessageDtos
		{
			get { return _createdMessageDtos.ToArray(); }
		}
	}
}
