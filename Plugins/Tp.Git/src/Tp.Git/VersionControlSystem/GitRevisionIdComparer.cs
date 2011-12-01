// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	public class GitRevisionIdComparer : IRevisionIdComparer, ICompareRevisionSecondArg
	{
		private GitRevisionId _firstArg;

		public ICompareRevisionSecondArg Is(RevisionId firstArg)
		{
			_firstArg = firstArg;
			return this;
		}

		public ICompareRevisionSecondArg Does(RevisionId firstArg)
		{
			_firstArg = firstArg;
			return this;
		}

		public RevisionId FindMinFromRevision(RevisionRange[] revisionRanges)
		{
			var result = (from revisionRange in revisionRanges orderby int.Parse(revisionRange.FromChangeset.Value) ascending select revisionRange).FirstOrDefault();
			if (result != null)
			{
				return result.FromChangeset;
			}
			return GitRevisionId.MinValue;
		}

		public RevisionId FindMaxToRevision(RevisionRange[] revisionRanges)
		{
			var result = (from revisionRange in revisionRanges orderby int.Parse(revisionRange.ToChangeset.Value) descending select revisionRange).FirstOrDefault();
			if (result != null)
			{
				return result.ToChangeset;
			}
			return GitRevisionId.MinValue;
		}

		public RevisionId ConvertToRevisionId(string startRevision)
		{
			return (GitRevisionId)(RevisionId)startRevision;
		}

		bool ICompareRevisionSecondArg.Before(RevisionRange revisionRange)
		{
			return _firstArg.Value < ((GitRevisionId) revisionRange.FromChangeset).Value;
		}

		bool ICompareRevisionSecondArg.Behind(RevisionRange revisionRange)
		{
			return _firstArg.Value > ((GitRevisionId) revisionRange.ToChangeset).Value;
		}

		bool ICompareRevisionSecondArg.Belong(RevisionRange revisionRange)
		{
			return _firstArg.Value >= ((GitRevisionId) revisionRange.FromChangeset).Value && _firstArg.Value <= ((GitRevisionId) revisionRange.ToChangeset).Value;
		}

		bool ICompareRevisionSecondArg.GreaterThan(RevisionId revisionId)
		{
			return _firstArg.Value > ((GitRevisionId) revisionId).Value;
		}

		bool ICompareRevisionSecondArg.LessThan(RevisionId revisionId)
		{
			return _firstArg.Value < ((GitRevisionId) revisionId).Value;
		}
	}
}