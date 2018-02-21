using System.Collections.Generic;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.RevisionStorage
{
    public interface IRevisionStorageRepository
    {
        ImportedRevisionInfo GetRevisionId(int? tpRevisionId);

        void SaveRevisionIdTpIdRelation(int tpRevisionId, RevisionId revisionId);

        IEnumerable<int> GetImportedTpIds(IEnumerable<int> tpRevisionIds);

        bool SaveRevisionInfo(RevisionInfo revision, out string key);

        void RemoveRevisionInfo(string revisionKey);

        string GetRevisionInfoKey(RevisionInfo revisionInfo);
    }
}
