using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateMessageCommand : SagaMessage, ITargetProcessCommand
	{
		public CreateMessageCommand()
		{
		}

		public CreateMessageCommand(MessageDTO dto)
		{
			Dto = dto;
		}

		public MessageDTO Dto { get; set; }
	}

	[Serializable]
	public class UpdateMessageCommand : UpdateEntityCommand<MessageDTO>
	{
		public UpdateMessageCommand(MessageDTO dto)
			: base(dto)
		{
		}

		public UpdateMessageCommand(MessageDTO dto, Enum[] changedFields)
			: base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteMessageCommand : DeleteEntityCommand<MessageDTO>
	{
		public DeleteMessageCommand(int id)
			: base(id)
		{
		}
	}

	[Serializable]
	public class AttachMessageToGeneralCommand : SagaMessage, ITargetProcessCommand
	{
		public int? MessageId { get; set; }
		public int? GeneralId { get; set; }
	}
}
