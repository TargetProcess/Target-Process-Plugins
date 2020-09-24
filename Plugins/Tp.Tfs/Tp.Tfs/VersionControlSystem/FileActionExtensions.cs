// 
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Tp.Integration.Common;

namespace Tp.Tfs.VersionControlSystem
{
    public static class TfvcChangeActionActionExtensions
    {
        public static FileActionEnum ToFileAction(this TfvcChange change)
        {
            if (change.ChangeType.HasFlag(VersionControlChangeType.Delete))
                return FileActionEnum.Delete;

            if (change.ChangeType.HasFlag(VersionControlChangeType.Add))
                return FileActionEnum.Add;

            if (change.ChangeType.HasFlag(VersionControlChangeType.Edit))
                return FileActionEnum.Modify;

            if (change.ChangeType.HasFlag(VersionControlChangeType.Rename))
                return FileActionEnum.Rename;

            if (change.ChangeType.HasFlag(VersionControlChangeType.Branch))
                return FileActionEnum.Branch;

            return FileActionEnum.None;
        }
    }
}
