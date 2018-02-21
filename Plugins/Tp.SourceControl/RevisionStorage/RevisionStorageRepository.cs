using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.RevisionStorage
{
    public abstract class RevisionStorageRepository : IRevisionStorageRepository
    {
        private readonly IStorageRepository _repository;
        private readonly IProfileCollectionReadonly _profiles;

        protected RevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles)
        {
            _repository = repository;
            _profiles = profiles;
        }

        protected IStorageRepository Repository
        {
            get { return _repository; }
        }

        public ImportedRevisionInfo GetRevisionId(int? tpRevisionId)
        {
            if (!tpRevisionId.HasValue)
            {
                return null;
            }

            return GetImportedRevisionIds(new[] { tpRevisionId.Value }).FirstOrDefault();
        }

        public virtual void SaveRevisionIdTpIdRelation(int tpRevisionId, RevisionId revisionId)
        {
            SaveRevisionsRelation(tpRevisionId, revisionId);
        }

        private void SaveRevisionsRelation(int tpRevisionId, RevisionId revisionId)
        {
            _repository.Get<RevisionIdRelation>(tpRevisionId.ToString())
                .ReplaceWith(new RevisionIdRelation { RevisionId = revisionId.Value, TpId = tpRevisionId });
        }

        public IEnumerable<int> GetImportedTpIds(IEnumerable<int> tpRevisionIds)
        {
            return GetImportedRevisionIds(tpRevisionIds)
                .Select(x => x.RevisionId.TpId)
                .ToList();
        }

        /*
         * Combination of ticks + comment used as revision info key so that duplicated commits are being ignored for importing
         * RevisionInfo Id can be duplicated
         * Example: cherry-picked commits in different branches
         */
        public virtual string GetRevisionInfoKey(RevisionInfo revisionInfo)
        {
            if (revisionInfo.TimeCreated.HasValue)
            {
                return TruncateRevisionInfoKey($"{revisionInfo.TimeCreated.Value.Ticks}#{revisionInfo.Comment}");
            }
            return revisionInfo.Id.Value;
        }

        private static string TruncateRevisionInfoKey(string revisionInfoKeyFull)
        {
            return revisionInfoKeyFull.Length > 255 ? revisionInfoKeyFull.Substring(0, 255) : revisionInfoKeyFull;
        }

        public virtual bool SaveRevisionInfo(RevisionInfo revision, out string key)
        {
            var revisionKey = GetRevisionInfoKey(revision);
            var info = Repository.Get<bool>(revisionKey);
            var revisionIsNew = !info.FirstOrDefault();

            if (revisionIsNew)
            {
                info.ReplaceWith(true);
            }

            key = revisionKey;

            return revisionIsNew;
        }

        public virtual void RemoveRevisionInfo(string revisionKey)
        {
            if (!string.IsNullOrEmpty(revisionKey))
            {
                Repository.Get<bool>(revisionKey).Remove(_ => true);
            }
        }

        protected virtual IEnumerable<ImportedRevisionInfo> GetImportedRevisionIds(IEnumerable<int> tpRevisionIds)
        {
            var storageNames = tpRevisionIds
                .Select(x => new StorageName(x.ToString()))
                .ToArray();

            return _profiles
                .Select(
                    profile =>
                        new
                        {
                            Revisions = profile.Get<RevisionIdRelation>(storageNames),
                            Profile = profile
                        })
                .SelectMany(
                    x => x.Revisions.Select(rev => new ImportedRevisionInfo { Profile = x.Profile, RevisionId = rev }))
                .ToList();
        }
    }
}
