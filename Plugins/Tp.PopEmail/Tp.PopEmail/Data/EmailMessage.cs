// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Integration.Common;
using Tp.Plugin.Core.Attachments;

namespace Tp.PopEmailIntegration.Data
{
	[Serializable]
	public class EmailMessage
	{
		public EmailMessage()
		{
			EmailAttachments = new List<LocalStoredAttachment>();
		}

		public string FromAddress { get; set; }
		public string FromDisplayName { get; set; }
		public string Recipients { get; set; }
		public List<MailAddressLite> CC { get; set; } 
		public string Subject { get; set; }
		public ContentTypeEnum ContentType { get; set; }
		public string Body { get; set; }
		public DateTime SendDate { get; set; }
		public List<LocalStoredAttachment> EmailAttachments { get; set; }
		public MessageUidDTO MessageUidDto { get; set; }

		public MessageDTO Convert()
		{
			return new MessageDTO
			       	{
			       		Recipients = Recipients,
			       		Subject = Subject,
			       		ContentType = ContentType,
			       		Body = Body,
			       		SendDate = SendDate,
			       		MessageType = MessageTypeEnum.Public,
			       		MessageUidDto = MessageUidDto
			       	};
		}
	}
}