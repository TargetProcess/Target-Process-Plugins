// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Bugzilla.Schemas;

namespace Tp.Bugzilla
{
	[Serializable]
	public class BugzillaComment
	{
		public BugzillaComment()
		{
		}

		public BugzillaComment(long_desc comment)
		{
			Body = comment.thetext;
			Author = comment.who;
			DateAdded = comment.bug_when;
		}

		public string Body { get; set; }

		public string Author { get; set; }

		public string DateAdded { get; set; }
	}
}