// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.Tfs.WorkItemsIntegration.FieldsMapping
{
	[DataContract]
	public class WorkItemField
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public string Value { get; set; }
	}
}
