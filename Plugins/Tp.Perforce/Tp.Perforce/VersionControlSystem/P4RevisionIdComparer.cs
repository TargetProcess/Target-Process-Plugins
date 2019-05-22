using System.Linq;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Perforce.VersionControlSystem
{
    public class P4RevisionIdComparer : IRevisionIdComparer, ICompareRevisionSecondArg
    {
        private P4RevisionId _firstArg;

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
            var revisions = (from revisionRange in revisionRanges select (P4RevisionId)revisionRange.ToChangeset);
            return (from revision in revisions orderby revision.Value descending select revision).First();
        }

        public RevisionId ConvertToRevisionId(string startRevision)
        {
            return startRevision;
        }

        public RevisionId FindMinFromRevision(RevisionRange[] revisionRanges)
        {
            var revisions = (from revisionRange in revisionRanges select (P4RevisionId)revisionRange.FromChangeset);
            return (from revision in revisions orderby revision.Value ascending select revision).First();
        }

        bool ICompareRevisionSecondArg.Before(RevisionRange revisionRange)
        {
            P4RevisionId fromChangeSet = revisionRange.FromChangeset;

            return _firstArg.Value < fromChangeSet.Value;
        }

        bool ICompareRevisionSecondArg.Behind(RevisionRange revisionRange)
        {
            P4RevisionId toChangeSet = revisionRange.ToChangeset;
            return _firstArg.Value > toChangeSet.Value;
        }

        bool ICompareRevisionSecondArg.Belong(RevisionRange revisionRange)
        {
            P4RevisionId fromChangeset = revisionRange.FromChangeset;
            P4RevisionId toChangeset = revisionRange.ToChangeset;
            return fromChangeset.Value <= _firstArg.Value && _firstArg.Value <= toChangeset.Value;
        }

        bool ICompareRevisionSecondArg.GreaterThan(RevisionId revisionId)
        {
            P4RevisionId svnRevisionId = revisionId;

            return _firstArg.Value > svnRevisionId.Value;
        }

        bool ICompareRevisionSecondArg.LessThan(RevisionId revisionId)
        {
            P4RevisionId svnRevisionId = revisionId;

            return _firstArg.Value < svnRevisionId.Value;
        }
    }
}