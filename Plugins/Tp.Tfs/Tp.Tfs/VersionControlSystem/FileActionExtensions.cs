// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.TeamFoundation.VersionControl.Client;
using Tp.Integration.Common;

namespace Tp.Tfs.VersionControlSystem
{
    public static class ChangesetActionActionExtensions
    {
        public static FileActionEnum ToFileAction(this Change change)
        {
            if (change.ChangeType.HasFlag(ChangeType.Delete))
                return FileActionEnum.Delete;

            if (change.ChangeType.HasFlag(ChangeType.Add))
                return FileActionEnum.Add;

            if (change.ChangeType.HasFlag(ChangeType.Edit))
                return FileActionEnum.Modify;

            if (change.ChangeType.HasFlag(ChangeType.Rename))
                return FileActionEnum.Rename;

            if (change.ChangeType.HasFlag(ChangeType.Branch))
                return FileActionEnum.Branch;

            return FileActionEnum.None;
        }
    }
}
