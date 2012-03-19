// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Xml.Serialization;

namespace Tp.Integration.Messages.Entities
{
	[Serializable]
	public class EmailAttachmentDTO
	{
		[XmlElement(Order = 3)]public byte[] Buffer { get; set; }

		[XmlElement(Order = 4)]public string Name { get; set; }
	}
}
