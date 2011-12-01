// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using NServiceBus;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateAttachmentCommand : CreateEntityCommand<AttachmentDTO>
	{
		public CreateAttachmentCommand(AttachmentDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateAttachmentCommand : UpdateEntityCommand<AttachmentDTO>
	{
		public UpdateAttachmentCommand(AttachmentDTO dto) : base(dto)
		{
		}

		public UpdateAttachmentCommand(AttachmentDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteAttachmentCommand : DeleteEntityCommand<AttachmentDTO>
	{
		public DeleteAttachmentCommand(int id) : base(id)
		{
		}
	}

	[Serializable]
	public class AddAttachmentPartToMessageCommand : ITargetProcessCommand
	{
		public DateTime? CreateDate { get; set; }
		public string BytesSerializedToBase64 { get; set; }
		public int? MessageId { get; set; }
		public int? GeneralId { get; set; }
		public bool IsLastPart { get; set; }
		public string FileName { get; set; }
		public string TPStorageFileName { get; set; }

		public int? OwnerId { get; set; }
		public string Description { get; set; }
	}

	[Serializable]
	public class AttachmentPartAddedMessage : SagaMessage, ISagaMessage
	{
		public string FileName { get; set; }
		public string TPStorageFileName { get; set; }
	}
}