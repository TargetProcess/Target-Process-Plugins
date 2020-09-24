//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using Microsoft.TeamFoundation.VersionControl.Client;

namespace Tp.Tfs.VersionControlSystem
{
    public static class ChangesetActionActionExtensions
    {
        public static Integration.Common.FileActionEnum ToFileAction(this Change change)
        {
            if (change.ChangeType.HasFlag(ChangeType.Delete))
                return Integration.Common.FileActionEnum.Delete;

            if (change.ChangeType.HasFlag(ChangeType.Add))
                return Integration.Common.FileActionEnum.Add;

            if (change.ChangeType.HasFlag(ChangeType.Edit))
                return Integration.Common.FileActionEnum.Modify;

            if (change.ChangeType.HasFlag(ChangeType.Rename))
                return Integration.Common.FileActionEnum.Rename;

            if (change.ChangeType.HasFlag(ChangeType.Branch))
                return Integration.Common.FileActionEnum.Branch;

            return Integration.Common.FileActionEnum.None;
        }
    }
}
