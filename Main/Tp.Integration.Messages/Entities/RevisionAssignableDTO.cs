﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Revision Assignable. Relation between assignable and revision.
	/// TargetProcess system usage only
    /// </summary>
	[Serializable]
	public partial class RevisionAssignableDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return RevisionAssignableID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				RevisionAssignableID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Revision Assignable ID.
        /// </summary>
        /// <value>The Revision Assignable ID.</value>
		[PrimaryKey]
		public int? RevisionAssignableID { get; set; }
		
		
		/// <summary>
        /// Gets or sets the Assignable ID. Reference to assignable
        /// </summary>
        /// <value>The Assignable ID.</value>
		[ForeignKey]
		public Int32? AssignableID { get; set; }
		
		/// <summary>
        /// Gets or sets the Revision ID. Reference to revision
        /// </summary>
        /// <value>The Revision ID.</value>
		[ForeignKey]
		public Int32? RevisionID { get; set; }
		

		
		/// <summary>
        /// Gets or sets the Assignable Name. Reference to assignable
        /// </summary>
        /// <value>The Assignable Name.</value>
		[RelationName]
		public virtual string AssignableName { get; set; }
		
	}
	
	
	/// <summary>
    /// Revision Assignable fields
    /// </summary>
	public enum RevisionAssignableField
	{
        /// <summary>
        /// Assignable ID
        /// </summary>		
		AssignableID,
        /// <summary>
        /// Revision ID
        /// </summary>		
		RevisionID,
        /// <summary>
        /// Assignable Name
        /// </summary>		
		AssignableName,
	}
}