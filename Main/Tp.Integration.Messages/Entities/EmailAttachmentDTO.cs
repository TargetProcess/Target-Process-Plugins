// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages.Entities
{
	[Serializable]
	public class EmailAttachmentDTO
	{
		public byte[] Buffer { get; set; }

		public string Name { get; set; }
	}
}