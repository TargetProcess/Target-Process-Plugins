// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Tfs.WorkItemsIntegration.FieldsMapping;

namespace Tp.Tfs.WorkItemsIntegration
{
	[Serializable]
	[DataContract]
	public class WorkItemInfo
	{
		[DataMember]
		public WorkItemId WorkItemId { get; set; }

		[DataMember]
		public TpEntityId TpEntityId { get; set; }

		[DataMember]
		public string WorkItemType { get; set; }

		[DataMember]
		public List<WorkItemField> FieldsValues { get; set; }

		//[DataMember]
		//public string Title { get; set; }

		//[DataMember]
		//public string Description { get; set; }

		[DataMember]
		public string TfsProject { get; set; }

		[DataMember]
		public string TpProjectName { get; set; }

		[DataMember]
		public int TpProjectId { get; set; }

		public Enum[] ChangedFields { get; set; }

		public WorkItemAction Action { get; set; }

		public override string ToString()
		{
			var workItemField = FieldsValues.FirstOrDefault(x => x.Name == "Title");
			return string.Format("{0}_{1}", WorkItemId.Id, workItemField == null ? string.Empty : workItemField.Value);
		}
	}
}
