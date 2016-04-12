// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tp.Bugzilla.Schemas;
using Tp.Core;

namespace Tp.Bugzilla.Tests
{
	public class BugzillaBugCollection : IEnumerable<bug>
	{
		private readonly List<bug> _bugs = new List<bug>();

		public IEnumerator<bug> GetEnumerator()
		{
			return _bugs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bug GetById(int bugId)
		{
			return _bugs.Single(x => x.bug_id == bugId.ToString());
		}

		public void Add(int bugId)
		{
			_bugs.Add(new bug {bug_id = bugId.ToString(), creation_ts = CurrentDate.Value.ToString("dd MMM yyyy HH':'mm")});
		}

		public void Add(int bugId, string name)
		{
			_bugs.Add(new bug { bug_id = bugId.ToString(), creation_ts = CurrentDate.Value.ToString("dd MMM yyyy HH':'mm"), short_desc = name});
		}

		public void Add(bug bug)
		{
			_bugs.Add(bug);
		}

		public void Add(int bugId, DateTime createdDate)
		{
			_bugs.Add(new bug {bug_id = bugId.ToString(), creation_ts = createdDate.ToString("dd MMM yyyy HH':'mm")});
		}

		public void SetBugDescription(int bugId, string bugDescription)
		{
			var bug = GetById(bugId);

			if (bug.long_descCollection == null)
			{
				bug.long_descCollection = new long_descCollection();
			}

			if (bug.long_descCollection.Count > 0)
			{
				bug.long_descCollection[0] = new long_desc {thetext = bugDescription};
			}
			else
			{
				bug.long_descCollection.Add(new long_desc {thetext = bugDescription});
			}
		}

		public void SetBugStatus(int bugId, string bugStatus)
		{
			GetById(bugId).bug_status = bugStatus;
		}

		public void SetBugSeverity(int bugId, string severity)
		{
			GetById(bugId).bug_severity = severity;
		}

		public void SetBugPriority(int bugId, string priority)
		{
			GetById(bugId).priority = priority;
		}

		public void SetBugCreationDate(int bugId, string creationDate)
		{
			GetById(bugId).creation_ts = creationDate;
		}

		public void SetBugReporter(int bugId, string reporterEmail)
		{
			GetById(bugId).reporter = reporterEmail;
		}

		public void SetBugAssignee(int bugId, string assingneeEmail)
		{
			GetById(bugId).assigned_to = assingneeEmail;
		}

		public void AddComment(int bugId, long_desc comment)
		{
			SetBugDescription(bugId, "description");

			var bug = GetById(bugId);
			bug.long_descCollection.Add(comment);
		}

		public void AddAttachment(int bugId, attachment attachment)
		{
			var bug = GetById(bugId);

			if (bug.attachmentCollection == null)
				bug.attachmentCollection = new attachmentCollection();

			bug.attachmentCollection.Add(attachment);
		}

		public void SetComponent(int bugId, string component)
		{
			GetById(bugId).component = component;
		}

		public void SetVersion(int bugId, string version)
		{
			GetById(bugId).version = version;
		}

		public void SetPlatform(int bugId, string platform)
		{
			GetById(bugId).rep_platform = platform;
		}

		public void SetOperatingSystem(int bugId, string os)
		{
			GetById(bugId).op_sys = os;
		}

		public void SetClassification(int bugId, string classification)
		{
			GetById(bugId).classification = classification;
		}

		public void SetCustomField(int bugId, string fieldName, string fieldValue)
		{
			var bug = EnsureCustomFields(bugId);

			bug.custom_fieldCollection.Add(new custom_field{cf_name = fieldName, cf_value = fieldValue});
		}
		
		public void SetCustomField(int bugId, string fieldName, string[] fieldValue)
		{
			var bug = EnsureCustomFields(bugId);

			var values = new cf_valueCollection();
			fieldValue.ToList().ForEach(v => values.Add(v));

			bug.custom_fieldCollection.Add(new custom_field
			                               	{
			                               		cf_name = fieldName, 
												cf_type = "Multiple-Selection Box",
												cf_values = new cf_values
												            	{
												            		cf_valueCollection = values
												            	}
			                               	});
		}

		private bug EnsureCustomFields(int bugId)
		{
			var bug = GetById(bugId);
			if(bug.custom_fieldCollection == null)
				bug.custom_fieldCollection = new custom_fieldCollection();

			return bug;
		}

		public void SetAttachmentOwner(int bugId, string fileName, string owner)
		{
			GetById(bugId).attachmentCollection.Cast<attachment>().Single(a => a.filename == fileName).attacher = owner;
		}

		public void SetAttachmentDescription(int bugId, string fileName, string description)
		{
			GetById(bugId).attachmentCollection.Cast<attachment>().Single(a => a.filename == fileName).desc = description;
		}

		public void SetAttachmentContent(int bugId, string fileName, data data)
		{
			GetById(bugId).attachmentCollection.Cast<attachment>().Single(a => a.filename == fileName).data = data;
		}

		public void SetAttachmentTime(int bugId, string fileName, string createDate)
		{
			GetById(bugId).attachmentCollection.Cast<attachment>().Single(a => a.filename == fileName).date = createDate;
		}
	}
}