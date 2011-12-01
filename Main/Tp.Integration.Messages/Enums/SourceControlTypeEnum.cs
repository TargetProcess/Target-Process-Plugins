// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Common
{
    /// <summary>
    /// Describes the types of Source Control
    /// </summary>
    public enum SourceControlTypeEnum
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None = 0,
        /// <summary>
        /// Subversion
        /// </summary>
        Subversion = 1,
        /// <summary>
        /// Source Safe
        /// </summary>
        SourceSafe = 2,
        /// <summary>
        /// Perforce
        /// </summary>
        Perforce = 3,
    }
}