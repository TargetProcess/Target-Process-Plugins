// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Tfs.VersionControlSystem
{
	public class TfsRevisionIdComparer : IRevisionIdComparer, ICompareRevisionSecondArg
	{
		private TfsRevisionId _firstArg;

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
			var result =
	(from revisionRange in revisionRanges orderby int.Parse(revisionRange.FromChangeset.Value) ascending select revisionRange).
		FirstOrDefault();

			return result != null ? result.FromChangeset : new RevisionId { Value = "1", Time = TfsRevisionId.UtcTimeMin };
		}

		public RevisionId FindMaxToRevision(RevisionRange[] revisionRanges)
		{
			var result =
				(from revisionRange in revisionRanges orderby int.Parse(revisionRange.ToChangeset.Value) descending select revisionRange).
					FirstOrDefault();

			return result != null ? result.ToChangeset : new RevisionId { Value = "1", Time = TfsRevisionId.UtcTimeMin };
		}

		public RevisionId ConvertToRevisionId(string startRevision)
		{
			var revisionId = (TfsRevisionId)(RevisionId)startRevision;

			Int32 startNumber;
			if (!Int32.TryParse(startRevision, out startNumber))
				throw new FormatException("Specify a start revision number in the range of 1 - 2147483647.");

			revisionId.Value = startNumber.ToString();

			return revisionId;
		}

		bool ICompareRevisionSecondArg.Before(RevisionRange revisionRange)
		{
			return int.Parse(_firstArg.Value) < int.Parse(revisionRange.FromChangeset.Value);
		}

		bool ICompareRevisionSecondArg.Behind(RevisionRange revisionRange)
		{
			return int.Parse(_firstArg.Value) > int.Parse(revisionRange.ToChangeset.Value);
		}

		bool ICompareRevisionSecondArg.Belong(RevisionRange revisionRange)
		{
			return int.Parse(_firstArg.Value) >= int.Parse(revisionRange.FromChangeset.Value) &&
						 int.Parse(_firstArg.Value) <= int.Parse(revisionRange.ToChangeset.Value);
		}

		bool ICompareRevisionSecondArg.GreaterThan(RevisionId revisionId)
		{
			return int.Parse(_firstArg.Value) > int.Parse(revisionId.Value);
		}

		bool ICompareRevisionSecondArg.LessThan(RevisionId revisionId)
		{
			return int.Parse(_firstArg.Value) < int.Parse(revisionId.Value);
		}
	}
}