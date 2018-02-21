// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.BugTracking.ImportToTp;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Bugzilla
{
    public interface IBugzillaInfoStorageRepository
    {
        TargetProcessBugId GetTargetProcessBugId(string bugzillaBugId);
        BugzillaBugInfo GetBugzillaBug(int? tpBugId);
        BugzillaBugInfo RemoveBugzillaBug(int? tpBugId);
        IEnumerable<BugzillaBugInfo> GetBugzillaBugs(IEnumerable<int> tpBugs);
        void SaveBugsRelation(int? tpBugId, BugzillaBugInfo bugzillaBug);
        void SaveBugzillaBugInfo(int? tpBugId, BugzillaBugInfo bug);
        IEnumerable<BugzillaBugInfo> GetAllBugzillaBugs(IEnumerable<int> tpBugs);
    }

    public class BugzillaInfoStorageRepository : IBugzillaInfoStorageRepository
    {
        private readonly IStorageRepository _repository;
        private readonly IProfileCollectionReadonly _profiles;

        public BugzillaInfoStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles)
        {
            _repository = repository;
            _profiles = profiles;
        }

        public IEnumerable<BugzillaBugInfo> GetAllBugzillaBugs(IEnumerable<int> tpBugs)
        {
            var storageNames = tpBugs
                .Select(x => new StorageName(x.ToString()))
                .ToArray();

            return _profiles
                .Select(profile => new
                {
                    Bugs = profile.Get<BugzillaBugInfo>(storageNames),
                    BugzillaUrl = GetBugzillaUrlForProfile(profile)
                })
                .SelectMany(x => x.Bugs.Select(bugInfo => SetBugUrl(bugInfo, x.BugzillaUrl)))
                .ToList();
        }

        public BugzillaBugInfo RemoveBugzillaBug(int? tpBugId)
        {
            var bugzillaBugInfo = GetBugzillaBug(tpBugId);
            if (bugzillaBugInfo != null)
            {
                _repository.Get<BugzillaBugInfo>(new StorageName(bugzillaBugInfo.TpId.ToString())).Clear();
            }
            return bugzillaBugInfo;
        }

        public IEnumerable<BugzillaBugInfo> GetBugzillaBugs(IEnumerable<int> tpBugs)
        {
            var storageNames = tpBugs
                .Select(x => new StorageName(x.ToString()))
                .ToArray();

            var bugzillaUrl = GetBugzillaUrlForProfile(_repository);

            return _repository.Get<BugzillaBugInfo>(storageNames).Select(x => SetBugUrl(x, bugzillaUrl));
        }

        private static BugzillaBugInfo SetBugUrl(BugzillaBugInfo bugInfo, BugzillaUrl bugzillaUrl)
        {
            bugInfo.Url = bugzillaUrl.GetBugExternalUrl(bugInfo.Id);
            return bugInfo;
        }

        private static BugzillaUrl GetBugzillaUrlForProfile(IStorageRepository profile)
        {
            return new BugzillaUrl(profile.GetProfile<BugzillaProfile>());
        }

        public void SaveBugsRelation(int? tpBugId, BugzillaBugInfo bugzillaBug)
        {
            _repository.Get<TargetProcessBugId>(bugzillaBug.Id).ReplaceWith(new TargetProcessBugId { Value = tpBugId.Value });

            SaveBugzillaBugInfo(tpBugId, bugzillaBug);
        }

        public TargetProcessBugId GetTargetProcessBugId(string bugzillaBugId)
        {
            return _repository.Get<TargetProcessBugId>(bugzillaBugId).SingleOrDefault();
        }

        public BugzillaBugInfo GetBugzillaBug(int? tpBugId)
        {
            if (!tpBugId.HasValue)
            {
                return null;
            }

            return GetBugzillaBugs(new[] { tpBugId.Value }).FirstOrDefault();
        }

        public void SaveBugzillaBugInfo(int? tpBugId, BugzillaBugInfo bug)
        {
            _repository.Get<BugzillaBugInfo>(tpBugId.GetValueOrDefault().ToString()).ReplaceWith(bug);
        }
    }
}
