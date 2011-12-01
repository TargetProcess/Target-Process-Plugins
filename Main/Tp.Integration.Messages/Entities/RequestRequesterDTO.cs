﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Request Requester. Represents Request Requester.
	/// TargetProcess system usage only
    /// </summary>
	[Serializable]
	public partial class RequestRequesterDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return RequestRequesterID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				RequestRequesterID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Request Requester ID.
        /// </summary>
        /// <value>The Request Requester ID.</value>
		[PrimaryKey]
		public int? RequestRequesterID { get; set; }
		
		
		/// <summary>
        /// Gets or sets the Request ID. Reference to request
        /// </summary>
        /// <value>The Request ID.</value>
		[ForeignKey]
		public Int32? RequestID { get; set; }
		
		/// <summary>
        /// Gets or sets the Requester ID. Reference to requester
        /// </summary>
        /// <value>The Requester ID.</value>
		[ForeignKey]
		public Int32? RequesterID { get; set; }
		

		
		/// <summary>
        /// Gets or sets the Request Name. Reference to request
        /// </summary>
        /// <value>The Request Name.</value>
		[RelationName]
		public virtual string RequestName { get; set; }
		
	}
	
	
	/// <summary>
    /// Request Requester fields
    /// </summary>
	public enum RequestRequesterField
	{
        /// <summary>
        /// Request ID
        /// </summary>		
		RequestID,
        /// <summary>
        /// Requester ID
        /// </summary>		
		RequesterID,
        /// <summary>
        /// Request Name
        /// </summary>		
		RequestName,
	}
}