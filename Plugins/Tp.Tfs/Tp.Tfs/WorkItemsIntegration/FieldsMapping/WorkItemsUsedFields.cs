// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Tp.Tfs.WorkItemsIntegration.FieldsMapping
{
	public static class WorkItemsUsedFields
	{
		private static readonly string[] BugFields = new[] { "Title", "Repro Steps", "Acceptance Criteria" };
		private static readonly string[] UserStoryFields = new[] { "Title", "Description" };
		private static readonly string[] IssueFields = new[] { "Title", "Description" };
		private static readonly string[] TaskFields = new[] { "Title", "Description" };
		private static readonly string[] BackLogFields = new[] { "Title", "Description", "Acceptance Criteria" };
		private static readonly string[] ImpedimentFields = new[] { "Title", "Description" };

		public static List<WorkItemField> Map(WorkItem workItem)
		{
			switch (workItem.Type.Name)
			{
				case Constants.TfsBug:
					{
						return GetMapping(workItem, BugFields);
					}
				case Constants.TfsTask:
					{
						return GetMapping(workItem, TaskFields);
					}
				case Constants.TfsUserStory:
					{
						return GetMapping(workItem, UserStoryFields);
					}
				case Constants.TfsIssue:
					{
						return GetMapping(workItem, IssueFields);
					}
				case Constants.TfsBacklogItem:
					{
						return GetMapping(workItem, BackLogFields);
					}
				case Constants.TfsImpediment:
					{
						return GetMapping(workItem, ImpedimentFields);
					}
				default:
					throw new ArgumentException(string.Format("Invalid Work Item Type: {0}", workItem.Type.Name));
			}
		}

		private static List<WorkItemField> GetMapping(WorkItem workItem, IEnumerable<string> fields)
		{
			return (from field in fields
			        where workItem.Fields.Contains(field)
			        let workItemField = workItem.Fields[field]
			        select new WorkItemField
				        {
					        Name = field, Type = workItemField.FieldDefinition.FieldType.ToString(), Value = workItemField.Value == null ? string.Empty : workItemField.Value.ToString()
				        }).ToList();
		}
	}
}
