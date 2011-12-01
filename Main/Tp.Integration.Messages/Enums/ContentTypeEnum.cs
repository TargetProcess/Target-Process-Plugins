// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Common
{
    /// <summary>
    /// Describes the content type of the Message
    /// </summary>
    public enum ContentTypeEnum
    {
        /// <summary>
        /// The content is undefined
        /// </summary>
        None = 0,
        /// <summary>
        /// The mail
        /// </summary>
        Mail = 1,
        /// <summary>
        /// The error
        /// </summary>
        Error = 2,
        /// <summary>
        /// The email
        /// </summary>
        Email = 3
    }
}