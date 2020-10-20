using System;

namespace Tp.Model.Common.Permissions
{
    [Flags]
    public enum EntityOperationType
    {
        None = 0,
        GrantAccess = 1 << 0,
        View = 1 << 1,
        Comment = 1 << 2,
        Edit = 1 << 3
    }
}
