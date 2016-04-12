using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
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

	[Serializable]
	public class CloneAttachmentCommand : SagaMessage, ITargetProcessCommand
	{
		public AttachmentDTO Dto { get; set; }
	}
}
