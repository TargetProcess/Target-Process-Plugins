﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Tag. Represents the tag.
    /// </summary>
	[Serializable]
	public partial class TagDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return TagID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				TagID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Tag ID.
        /// </summary>
        /// <value>The Tag ID.</value>
		[PrimaryKey]
		public int? TagID { get; set; }
		

		/// <summary>
        /// Gets or sets the Name. The name of the tag
        /// </summary>
        /// <value>The Name.</value>
		public String Name { get; set; }
		

		
	}
	
	
	/// <summary>
    /// Tag fields
    /// </summary>
	public enum TagField
	{
        /// <summary>
        /// Name
        /// </summary>		
		Name,
	}
}