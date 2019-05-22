using System.Collections.Generic;
using System.Linq;
using Perforce.P4;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Perforce.VersionControlSystem
{
    public static class P4Utils
    {
        private static readonly IActivityLogger _log = ObjectFactory.GetInstance<IActivityLogger>();

        public static RevisionInfo[] ArrayOfSvnRevisionToArrayOfRevisionInfo(IList<Changelist> p4Revisions,
            IVersionControlSystem versionControlSystem)
        {
            var tpRevisionInfo = new List<RevisionInfo>(p4Revisions.Count);
            _log.Info("Process Perforce revisions...");
            foreach (var p4Revision in p4Revisions)
            {
                _log.Info($"Do processing of changelist {p4Revision.Id}");

                if (p4Revision.Id == 0 ||
                    p4Revision.Files == null ||
                    p4Revision.Files.Count == 0)
                {
                    _log.Info(
                        $"Skip processing changelist {p4Revision.Id}. {(p4Revision.Files == null ? "p4Revision.Files == null" : $"p4Revision.Files.Count == {p4Revision.Files.Count}")}.");
                    continue;
                }

                _log.Info("Adding revisions infos");

                var info = P4RevisionToRevisionInfo(p4Revision, versionControlSystem);
                tpRevisionInfo.Add(info);
            }
            return tpRevisionInfo.ToArray();
        }

        private static RevisionInfo P4RevisionToRevisionInfo(Changelist changelist, IVersionControlSystem vcs)
        {
            var infos = P4ChangeItemCollectionToArrayOfRevisionEntryInfo(changelist.Files);
            var entries = infos.ToArray();

            _log.Info(
                $"Revision Info ctor: {changelist.Id},{changelist.OwnerName},{changelist.Description},{changelist.ModifiedDate.ToLocalTime()},{entries}");
            return new RevisionInfo
            {
                Id = changelist.Id.ToString(),
                Author = changelist.OwnerName,
                Comment = changelist.Description,
                Time = changelist.ModifiedDate.ToLocalTime(),
                Entries = entries
            };
        }

        private static List<RevisionEntryInfo> P4ChangeItemCollectionToArrayOfRevisionEntryInfo(ICollection<FileMetaData> collection)
        {
            var revisionEntryInfoList = new List<RevisionEntryInfo>();
            if (collection != null)
            {
                revisionEntryInfoList.AddRange(collection.Select(fileMetaData => new RevisionEntryInfo
                {
                    Path = fileMetaData.DepotPath.Path,
                    Action = P4ChangeActionToFileActionEnum(fileMetaData.Action)
                }));

            }
            return revisionEntryInfoList;
        }

        private static FileActionEnum P4ChangeActionToFileActionEnum(FileAction action)
        {
            var fileAction = FileActionEnum.None;
            switch (action)
            {
                case FileAction.Add:
                case FileAction.Added:
                    fileAction = FileActionEnum.Add;
                    break;

                case FileAction.Delete:
                case FileAction.MoveDelete:
                    fileAction = FileActionEnum.Delete;
                    break;

                case FileAction.Edit:
                case FileAction.Updated:
                    fileAction = FileActionEnum.Modify;
                    break;

                case FileAction.MoveAdd:
                    fileAction = FileActionEnum.Rename;
                    break;
            }
            return fileAction;
        }
    }
}
