using System;
using System.Runtime.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class AttachmentPartAddedMessage : SagaMessage, ISagaMessage
	{
	}

	[Serializable]
	public class EntityStatePartAddedMessage : SagaMessage, ISagaMessage
	{
	}

	[Serializable]
	public class MessageAttachedToGeneralMessage : SagaMessage, ISagaMessage
	{
		public int? MessageId { get; set; }
		public int? GeneralId { get; set; }
	}

	[Serializable]
	public class MessageWithTheSameUidExistsException : Exception
	{
		public MessageWithTheSameUidExistsException()
		{
		}

		public MessageWithTheSameUidExistsException(string message)
			: base(message)
		{
		}

		public MessageWithTheSameUidExistsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected MessageWithTheSameUidExistsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	[Serializable]
	public class PriorityPartAddedMessage : SagaMessage, ISagaMessage
	{
	}

	[Serializable]
	public class GeneralUserAttachedToRequestMessage : SagaMessage, ISagaMessage
	{
		public int? RequesterId { get; set; }
		public int? RequestId { get; set; }
	}

	[Serializable]
	public class SeverityPartAddedMessage : SagaMessage, ISagaMessage
	{
	}

	[Serializable]
	public class TestCaseToTestPlanAddedMessage : EntityCreatedMessage<TestCaseTestPlanDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TestCaseFromTestPlanDeletedMessage : EntityDeletedMessage<TestCaseTestPlanDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TestCaseInTestPlanUpdatedMessage : EntityUpdatedMessage<TestCaseTestPlanDTO, TestCaseTestPlanField>, ISagaMessage
	{
	}
}
