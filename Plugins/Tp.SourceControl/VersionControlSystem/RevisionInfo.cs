// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.SourceControl.VersionControlSystem
{
	[Serializable]
	public class RevisionInfo
	{
		public RevisionInfo()
		{
			Entries = new RevisionEntryInfo[] {};
			Time = DateTime.Now;
			Comment = string.Empty;
		}

		public RevisionId Id { get; set; }

		public string Comment { get; set; }

		public string Author { get; set; }

		public string Email { get; set; }

		public DateTime Time { get; set; }

		public DateTime? TimeCreated { get; set; }

		public RevisionEntryInfo[] Entries { get; set; }
	}

	public class RevisionEntryInfo
	{
		public string Path { get; set; }
		public FileActionEnum Action { get; set; }
	}
}