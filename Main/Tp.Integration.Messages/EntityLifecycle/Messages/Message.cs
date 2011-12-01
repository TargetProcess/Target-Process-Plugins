// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Runtime.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class MessageCreatedMessage : EntityCreatedMessage<MessageDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class MessageDeletedMessage : EntityDeletedMessage<MessageDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class MessageUpdatedMessage : EntityUpdatedMessage<MessageDTO, MessageField>, ISagaMessage
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

		public MessageWithTheSameUidExistsException(string message) : base(message)
		{
		}

		public MessageWithTheSameUidExistsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MessageWithTheSameUidExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}