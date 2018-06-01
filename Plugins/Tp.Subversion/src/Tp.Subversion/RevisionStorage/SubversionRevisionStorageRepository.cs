// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.RevisionStorage
{
    public class SubversionRevisionStorageRepository : RevisionStorageRepository
    {
        public SubversionRevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles) : base(repository, profiles) { }

        public override string GetRevisionInfoKey(RevisionInfo revisionInfo)
        {
            return revisionInfo.Id.Value;
        }

        public override bool SaveRevisionInfo(RevisionInfo revision, out string key)
        {
            key = GetRevisionInfoKey(revision);
            return true;
        }

        public override void RemoveRevisionInfo(string revisionKey)
        {
            //do nothing
        }
    }
}
