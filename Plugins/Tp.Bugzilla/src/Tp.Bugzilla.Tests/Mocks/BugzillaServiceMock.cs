// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using StructureMap;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Tests.Mocks
{
	public class BugzillaServiceMock : IBugzillaService
	{
		//private readonly BugzillaBugCollection _bugs = new BugzillaBugCollection();
		private readonly List<string> _statuses = new List<string>();
		private readonly List<string> _resolutions = new List<string>();
		private readonly Dictionary<string, int> _bugUpdateCalls = new Dictionary<string, int>();
		private TimeSpan _timeOffset;
		private readonly IDictionary<ProfileName, BugzillaBugCollection> _bugsCollection = new Dictionary<ProfileName, BugzillaBugCollection>();

		public IProfileReadonly InnerProfile { get; set; }

		public IProfileReadonly CurrentProfile
		{
			get
			{
				return InnerProfile ?? ObjectFactory.GetInstance<IProfileReadonly>();
			}
		}

		public BugzillaBugCollection Bugs
		{
			get
			{
				if (!_bugsCollection.ContainsKey(CurrentProfile.Name))
				{
					_bugsCollection.Add(CurrentProfile.Name, new BugzillaBugCollection());
				}
				return _bugsCollection[CurrentProfile.Name];
			}
		}

		public List<string> Statuses
		{
			get { return _statuses; }
		}

		public List<string> Resolutions
		{
			get { return _resolutions; }
		}

		public Dictionary<string, int> BugUpdateCalls
		{
			get { return _bugUpdateCalls; }
		}

		public virtual int[] GetChangedBugIds(DateTime lastSyncDate)
		{
			return Bugs.Where(x => DateTime.Parse(x.creation_ts) > lastSyncDate).Select(x => int.Parse(x.bug_id)).ToArray();
		}

		public bugzilla_properties CheckConnection()
		{
			throw new NotImplementedException();
		}

		public virtual bugCollection GetBugs(int[] bugIDs)
		{
			var bugCollection = new bugCollection();
			bugCollection.AddRange(Bugs.Distinct(new BugComparer()).Where(x => bugIDs.Contains(int.Parse(x.bug_id))).Select(x => x).ToArray());
			return bugCollection;
		}

		public List<string> GetStatuses()
		{
			return Statuses;
		}

		public List<string> GetResolutions()
		{
			return Resolutions;
		}

		public void Execute(IBugzillaQuery query)
		{
			if(query is BugzillaCommentAction)
			{
				SendComment(query as BugzillaCommentAction);
			}
			else if(query is BugzillaAssigneeAction)
			{
				AssignUser(query as BugzillaAssigneeAction);
			}
			else
			{
				ChangeState(query as BugzillaChangeStatusAction);
			}
		}

		private void SendComment(BugzillaCommentAction bugzillaCommentAction)
		{
			var queryString = HttpUtility.ParseQueryString(bugzillaCommentAction.Value());

			var bug = Bugs.GetById(int.Parse(queryString["bugid"]));

			if(bug.long_descCollection == null || bug.long_descCollection.Count < 1)
			{
				bug.long_descCollection = new long_descCollection {new long_desc {thetext = "description"}};
			}

			bug.long_descCollection.Add(new long_desc
			                            	{
			                            		thetext = Encoding.ASCII.GetString(Convert.FromBase64String(queryString["comment_text"])), 
												who = queryString["owner"],
												bug_when = queryString["date"]
			                            	});
		}

		private void AssignUser(BugzillaAssigneeAction assigneeAction)
		{
			var queryString = HttpUtility.ParseQueryString(assigneeAction.Value());

			var bug = Bugs.GetById(int.Parse(queryString["bugid"]));

			bug.assigned_to = queryString["user"];
		}

		private void ChangeState(BugzillaChangeStatusAction action)
		{
			var queryString = HttpUtility.ParseQueryString(action.Value());

			var bug = Bugs.GetById(int.Parse(queryString["id"]));
			bug.bug_status = queryString["status"];
			if (!string.IsNullOrEmpty(queryString["resolution"]))
				bug.resolution = queryString["resolution"];

			if (!string.IsNullOrEmpty(queryString["dup_id"]))
				bug.dup_id = queryString["dup_id"];

			if (!BugUpdateCalls.ContainsKey(bug.bug_id))
			{
				BugUpdateCalls.Add(bug.bug_id, 0);
			}
			BugUpdateCalls[bug.bug_id]++;
		}

		public TimeSpan GetTimeOffset()
		{
			return _timeOffset;
		}

		public void SetTimeOffset(TimeSpan offset)
		{
			var date = DateTime.Now;
			var currentZoneOffset = date - date.ToUniversalTime();

			_timeOffset = offset + currentZoneOffset;
		}
	}

	public class BugComparer : IEqualityComparer<bug>
	{
		public bool Equals(bug x, bug y)
		{
			return x.bug_id == y.bug_id;
		}

		public int GetHashCode(bug obj)
		{
			return obj.bug_id.GetHashCode();
		}
	}
}
