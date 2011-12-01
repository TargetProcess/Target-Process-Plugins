﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Tp Event. Represents event defined for some entity type.
	/// TargetProcess system usage only
    /// </summary>
	[Serializable]
	public partial class TpEventDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return TpEventID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				TpEventID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Tp Event ID.
        /// </summary>
        /// <value>The Tp Event ID.</value>
		[PrimaryKey]
		public int? TpEventID { get; set; }
		

		/// <summary>
        /// Gets or sets the Action Type. The type of action
        /// </summary>
        /// <value>The Action Type.</value>
		public ActionTypeEnum? ActionType { get; set; }

		/// <summary>
        /// Gets or sets the Entity Type. Reference to issued entity type. For example Bug
        /// </summary>
        /// <value>The Entity Type.</value>
		public String EntityType { get; set; }
		
		/// <summary>
        /// Gets or sets the State ID. Reference to issued state
        /// </summary>
        /// <value>The State ID.</value>
		[ForeignKey]
		public Int32? EntityStateID { get; set; }
		

		
		/// <summary>
        /// Gets or sets the State Name. Reference to issued state
        /// </summary>
        /// <value>The State Name.</value>
		[RelationName]
		public virtual string StateName { get; set; }
		
	}
	
	
	/// <summary>
    /// Tp Event fields
    /// </summary>
	public enum TpEventField
	{
        /// <summary>
        /// Action Type
        /// </summary>		
		ActionType,
        /// <summary>
        /// Entity Type
        /// </summary>		
		EntityType,
        /// <summary>
        /// Entity State ID
        /// </summary>		
		EntityStateID,
        /// <summary>
        /// State Name
        /// </summary>		
		StateName,
	}
}