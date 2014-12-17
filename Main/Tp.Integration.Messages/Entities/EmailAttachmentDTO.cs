// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Xml.Serialization;using System.Runtime.Serialization;

namespace Tp.Integration.Common
{
	[Serializable][DataContract]
	public partial class EmailAttachmentDTO : DataTransferObject
	{
		[DataMember][XmlElement(Order = 3)]public byte[] Buffer { get; set; }

		[DataMember][XmlElement(Order = 4)]public string Name { get; set; }
	}
}
