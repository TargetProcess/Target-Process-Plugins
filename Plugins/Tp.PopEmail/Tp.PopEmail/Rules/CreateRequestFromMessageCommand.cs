// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.PopEmailIntegration.Rules
{
	[Serializable]
	public class CreateRequestFromMessageCommand : IPluginLocalMessage
	{
		public MessageDTO MessageDto { get; set; }
		public int ProjectId { get; set; }
		public AttachmentDTO[] Attachments { get; set; }
	}
}