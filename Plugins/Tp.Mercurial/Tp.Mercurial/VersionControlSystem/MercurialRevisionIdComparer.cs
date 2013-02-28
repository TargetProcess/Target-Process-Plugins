// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Mercurial.VersionControlSystem
{
	public class MercurialRevisionIdComparer : IRevisionIdComparer, ICompareRevisionSecondArg
	{
		private MercurialRevisionId _firstArg;

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
				(from revisionRange in revisionRanges orderby revisionRange.FromChangeset.Time ascending select revisionRange).
					FirstOrDefault();
			
			return result != null ? result.FromChangeset : new RevisionId {Time = MercurialRevisionId.UtcTimeMin};
		}

		public RevisionId FindMaxToRevision(RevisionRange[] revisionRanges)
		{
			var result =
				(from revisionRange in revisionRanges orderby revisionRange.ToChangeset.Time descending select revisionRange).
					FirstOrDefault();

            return result != null ? result.ToChangeset : new RevisionId { Time = MercurialRevisionId.UtcTimeMin };
		}

		public RevisionId ConvertToRevisionId(string startRevision)
		{
            var revisionId = (MercurialRevisionId)(RevisionId)startRevision;

		    DateTime startDate;
            if (!DateTime.TryParse(startRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out startDate))
                throw new FormatException("StartRevision argument is in invalid datetime format.");

		    revisionId.Time = startDate;

			return revisionId;
		}

		bool ICompareRevisionSecondArg.Before(RevisionRange revisionRange)
		{
            return _firstArg.Time < ((MercurialRevisionId)revisionRange.FromChangeset).Time;
		}

		bool ICompareRevisionSecondArg.Behind(RevisionRange revisionRange)
		{
            return _firstArg.Time > ((MercurialRevisionId)revisionRange.ToChangeset).Time;
		}

		bool ICompareRevisionSecondArg.Belong(RevisionRange revisionRange)
		{
            return _firstArg.Time >= ((MercurialRevisionId)revisionRange.FromChangeset).Time &&
                   _firstArg.Time <= ((MercurialRevisionId)revisionRange.ToChangeset).Time;
		}

		bool ICompareRevisionSecondArg.GreaterThan(RevisionId revisionId)
		{
            return _firstArg.Time > ((MercurialRevisionId)revisionId).Time;
		}

		bool ICompareRevisionSecondArg.LessThan(RevisionId revisionId)
		{
            return _firstArg.Time < ((MercurialRevisionId)revisionId).Time;
		}
	}
}