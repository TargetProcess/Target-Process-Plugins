// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Common
{
    /// <summary>
    /// Describes the type of the Custom Field
    /// </summary>
    public enum FieldTypeEnum
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None = -1,
        /// <summary>
        /// The text
        /// </summary>
        Text = 0,
        /// <summary>
        /// The drop down
        /// </summary>
        DropDown = 1,
        /// <summary>
        /// The check box
        /// </summary>
        CheckBox = 2,
        /// <summary>
        /// URL
        /// </summary>
        URL = 3,
        /// <summary>
        /// The date
        /// </summary>
        Date = 4,
        /// <summary>
        /// The rich text
        /// </summary>
        RichText = 5,
        ///<summary>
        /// The number
        ///</summary>
        Number = 6,
        /// <summary>
        /// Target Process Entity
        /// </summary>
        Entity = 7
        
    }
}