// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpSvn;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.Subversion
{
    public static class SubversionUtils
    {
        private static readonly IActivityLogger _log = ObjectFactory.GetInstance<IActivityLogger>();

        public static RevisionInfo[] ArrayOfSvnRevisionToArrayOfRevisionInfo(Collection<SvnLogEventArgs> svnRevisions,
            IVersionControlSystem versionControlSystem)
        {
            var tpRevisionInfo = new List<RevisionInfo>(svnRevisions.Count);
            _log.InfoFormat("Process svn revisions...");
            foreach (var svnRevision in svnRevisions)
            {
                _log.Info($"Do processing of revision {svnRevision.Revision}");

                if (svnRevision.Revision == 0 ||
                    svnRevision.ChangedPaths == null ||
                    svnRevision.ChangedPaths.Count == 0)
                {
                    _log.Info(
                        $"Skip processing revision {svnRevision.Revision}. {(svnRevision.ChangedPaths == null ? "ChangedPaths == null" : $"ChangedPaths.Count == {svnRevision.ChangedPaths.Count}")}.");
                    continue;
                }

                _log.Info("Adding revisions infos");

                var info = SvnRevisionToRevisionInfo(svnRevision, versionControlSystem);
                tpRevisionInfo.Add(info);
            }
            return tpRevisionInfo.ToArray();
        }

        private static RevisionInfo SvnRevisionToRevisionInfo(SvnLoggingEventArgs revision, IVersionControlSystem vcs)
        {
            var infos = SvnChangeItemCollectionToArrayOfRevisionEntryInfo(revision.ChangedPaths);
            var entries = infos.ToArray();

            _log.Info($"Revision Info ctor: {revision.Revision},{revision.Author},{revision.LogMessage},{revision.Time.ToLocalTime()},{entries}");
            return new RevisionInfo
            {
                Id = revision.Revision.ToString(),
                Author = revision.Author,
                Comment = revision.LogMessage,
                Time = revision.Time.ToLocalTime(),
                Entries = entries
            };
        }

        private static List<RevisionEntryInfo> SvnChangeItemCollectionToArrayOfRevisionEntryInfo(
            ICollection<SvnChangeItem> collection)
        {
            if (collection == null)
            {
                return null;
            }

            var revisionEntryInfoList = new List<RevisionEntryInfo>(collection.Count);

            foreach (var info in collection)
            {
                var item = new RevisionEntryInfo { Path = info.Path, Action = SvnChangeActionToFileActionEnum(info.Action) };
                revisionEntryInfoList.Add(item);
            }

            return revisionEntryInfoList;
        }

        private static FileActionEnum SvnChangeActionToFileActionEnum(SvnChangeAction action)
        {
            var fileAction = FileActionEnum.None;
            switch (action)
            {
                case SvnChangeAction.Add:
                    fileAction = FileActionEnum.Add;
                    break;

                case SvnChangeAction.Delete:
                    fileAction = FileActionEnum.Delete;
                    break;

                case SvnChangeAction.Modify:
                    fileAction = FileActionEnum.Modify;
                    break;

                case SvnChangeAction.Replace:
                    fileAction = FileActionEnum.Rename;
                    break;
            }
            return fileAction;
        }
    }
}
