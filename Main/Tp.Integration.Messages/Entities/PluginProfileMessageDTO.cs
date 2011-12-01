﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Plugin Profile Message. 
    /// </summary>
	[Serializable]
	public partial class PluginProfileMessageDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return PluginProfileMessageID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				PluginProfileMessageID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Plugin Profile Message ID.
        /// </summary>
        /// <value>The Plugin Profile Message ID.</value>
		[PrimaryKey]
		public int? PluginProfileMessageID { get; set; }
		

		/// <summary>
        /// Gets or sets the Create Date. 
        /// </summary>
        /// <value>The Create Date.</value>
		public DateTime? CreateDate { get; set; }

		/// <summary>
        /// Gets or sets the Successfull. 
        /// </summary>
        /// <value>The Successfull.</value>
		public Boolean? Successfull { get; set; }

		/// <summary>
        /// Gets or sets the Message. 
        /// </summary>
        /// <value>The Message.</value>
		public String Message { get; set; }

		/// <summary>
        /// Gets or sets the Plugin Profile ID. 
        /// </summary>
        /// <value>The Plugin Profile ID.</value>
		public Int32? PluginProfileID { get; set; }
		

		
	}
	
	
	/// <summary>
    /// Plugin Profile Message fields
    /// </summary>
	public enum PluginProfileMessageField
	{
        /// <summary>
        /// Create Date
        /// </summary>		
		CreateDate,
        /// <summary>
        /// Successfull
        /// </summary>		
		Successfull,
        /// <summary>
        /// Message
        /// </summary>		
		Message,
        /// <summary>
        /// Plugin Profile ID
        /// </summary>		
		PluginProfileID,
	}
}