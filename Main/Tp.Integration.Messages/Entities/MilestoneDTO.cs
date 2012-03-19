// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Xml.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
	public class MilestoneDTO : DataTransferObject
	{
		[XmlElement(Order = 3)]public int? MilestoneId { get; set; }

		[XmlElement(Order = 4)]public string Name { get; set; }

		[XmlElement(Order = 5)]public string Description { get; set; }

		[XmlElement(Order = 6)]public DateTime Date { get; set; }

		[XmlElement(Order = 7)]public string CssClass { get; set; }

		public override int? ID
		{
			get { return MilestoneId; }
			set { MilestoneId = value; }
		}
	}

	public enum MilestoneField
	{
		Name,
		Description,
		Date,
		OwnerID,
		CssClass
	}
}
