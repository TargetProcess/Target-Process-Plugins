// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NGit;
using NGit.Diff;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Tp.Integration.Common;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	public static class RevCommitExtensions
	{
		public static RevisionInfo ConvertToRevisionInfo(this RevCommit commit, Repository repository)
		{
			var authorIdent = commit.GetAuthorIdent();
			DateTime commitTime = authorIdent.GetWhen();
			return new RevisionInfo
			{
				Author = authorIdent.GetName(),
				Comment = commit.GetFullMessage().TrimEnd('\n'),
				Id = (GitRevisionId)commitTime,
				Time = commitTime,
				Entries = commit.GetEntriesEnc(repository)
			};
		}

		private static RevisionEntryInfo[] GetEntriesEnc(this RevCommit commit, Repository repository)
		{
			var tw = new TreeWalk(repository) {Recursive = true};

			if (commit.ParentCount > 0)
			{
				tw.AddTree(commit.Parents.First().Tree);
			}
			else
			{
				tw.AddTree(new EmptyTreeIterator());
			}

			tw.Filter = TreeFilter.ANY_DIFF;

			tw.AddTree(commit.Tree);

			var differences = DiffEntry.Scan(tw);

			return differences.Select(ConvertToRevisionEntryInfo).ToArray();
		}

		private static RevisionEntryInfo ConvertToRevisionEntryInfo(DiffEntry difference)
		{
			var path = Path(difference);
			var revisionEntryInfo = new RevisionEntryInfo {Path = path, Action = GetAction(difference.GetChangeType())};

			return revisionEntryInfo;
		}

		private static string Path(DiffEntry difference)
		{
			switch (difference.GetChangeType())
			{
				case DiffEntry.ChangeType.ADD:
					return difference.GetNewPath();

				case DiffEntry.ChangeType.COPY:
					return string.Format("{0} -> {1}", difference.GetOldPath(), difference.GetNewPath());

				case DiffEntry.ChangeType.DELETE:
					return difference.GetOldPath();

				case DiffEntry.ChangeType.MODIFY:
					return difference.GetOldPath();

				case DiffEntry.ChangeType.RENAME:
					return string.Format("{0} -> {1}", difference.GetOldPath(), difference.GetNewPath());

				default:
					return difference.ToString();
			}
		}

		private static FileActionEnum GetAction(DiffEntry.ChangeType changeType)
		{
			switch (changeType)
			{
				case DiffEntry.ChangeType.MODIFY:
				case DiffEntry.ChangeType.COPY:
					return FileActionEnum.Modify;

				case DiffEntry.ChangeType.ADD:
					return FileActionEnum.Add;

				case DiffEntry.ChangeType.DELETE:
					return FileActionEnum.Delete;

				case DiffEntry.ChangeType.RENAME:
					return FileActionEnum.Rename;
				default:
					return FileActionEnum.None;
			}
		}
	}
}