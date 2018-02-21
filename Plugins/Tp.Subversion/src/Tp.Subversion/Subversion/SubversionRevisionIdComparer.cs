// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.Subversion
{
    public class SubversionRevisionIdComparer : IRevisionIdComparer, ICompareRevisionSecondArg
    {
        private SvnRevisionId _firstArg;

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

        public RevisionId FindMaxToRevision(RevisionRange[] revisionRanges)
        {
            var revisions = (from revisionRange in revisionRanges select (SvnRevisionId) revisionRange.ToChangeset);
            return (from revision in revisions orderby revision.Value descending select revision).First();
        }

        public RevisionId ConvertToRevisionId(string startRevision)
        {
            return startRevision;
        }

        public RevisionId FindMinFromRevision(RevisionRange[] revisionRanges)
        {
            var revisions = (from revisionRange in revisionRanges select (SvnRevisionId) revisionRange.FromChangeset);
            return (from revision in revisions orderby revision.Value ascending select revision).First();
        }

        bool ICompareRevisionSecondArg.Before(RevisionRange revisionRange)
        {
            SvnRevisionId fromChangeSet = revisionRange.FromChangeset;

            return _firstArg.Value < fromChangeSet.Value;
        }

        bool ICompareRevisionSecondArg.Behind(RevisionRange revisionRange)
        {
            SvnRevisionId toChangeSet = revisionRange.ToChangeset;
            return _firstArg.Value > toChangeSet.Value;
        }

        bool ICompareRevisionSecondArg.Belong(RevisionRange revisionRange)
        {
            SvnRevisionId fromChangeset = revisionRange.FromChangeset;
            SvnRevisionId toChangeset = revisionRange.ToChangeset;
            return fromChangeset.Value <= _firstArg.Value && _firstArg.Value <= toChangeset.Value;
        }

        bool ICompareRevisionSecondArg.GreaterThan(RevisionId revisionId)
        {
            SvnRevisionId svnRevisionId = revisionId;

            return _firstArg.Value > svnRevisionId.Value;
        }

        bool ICompareRevisionSecondArg.LessThan(RevisionId revisionId)
        {
            SvnRevisionId svnRevisionId = revisionId;

            return _firstArg.Value < svnRevisionId.Value;
        }
    }
}
