using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercurial;
using Tp.Integration.Common;

namespace Tp.Mercurial.VersionControlSystem
{
    public static class ChangesetActionActionExtensions
    {
        public static FileActionEnum ToFileAction(this ChangesetPathActionType action)
        {
            switch (action)
            {
                case ChangesetPathActionType.Add:
                    return FileActionEnum.Add;
                case ChangesetPathActionType.Modify:
                    return FileActionEnum.Modify;
                case ChangesetPathActionType.Remove:
                    return FileActionEnum.Delete;

                default:
                    return FileActionEnum.None;
            }
        }
    }
}
