// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
	public class MilestoneDTO : DataTransferObject
	{
		public int? MilestoneId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public DateTime Date { get; set; }

		public string CssClass { get; set; }

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