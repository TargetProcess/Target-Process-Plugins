﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Contact. 
    /// </summary>
	[Serializable]
	public partial class ContactDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return UserID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				UserID = value;
			}
		}

        /// <summary>
        /// Gets or sets the User ID.
        /// </summary>
        /// <value>The User ID.</value>
		[PrimaryKey]
		public int? UserID { get; set; }
		

		/// <summary>
        /// Gets or sets the First Name. 
        /// </summary>
        /// <value>The First Name.</value>
		public String FirstName { get; set; }

		/// <summary>
        /// Gets or sets the Last Name. 
        /// </summary>
        /// <value>The Last Name.</value>
		public String LastName { get; set; }

		/// <summary>
        /// Gets or sets the Email. 
        /// </summary>
        /// <value>The Email.</value>
		public String Email { get; set; }

		/// <summary>
        /// Gets or sets the Login. 
        /// </summary>
        /// <value>The Login.</value>
		public String Login { get; set; }

		/// <summary>
        /// Gets or sets the Password. 
        /// </summary>
        /// <value>The Password.</value>
		public String Password { get; set; }

		/// <summary>
        /// Gets or sets the Create Date. 
        /// </summary>
        /// <value>The Create Date.</value>
		public DateTime? CreateDate { get; set; }

		/// <summary>
        /// Gets or sets the Modify Date. 
        /// </summary>
        /// <value>The Modify Date.</value>
		public DateTime? ModifyDate { get; set; }

		/// <summary>
        /// Gets or sets the Delete Date. 
        /// </summary>
        /// <value>The Delete Date.</value>
		public DateTime? DeleteDate { get; set; }

		/// <summary>
        /// Gets or sets the Is Active. 
        /// </summary>
        /// <value>The Is Active.</value>
		public Boolean? IsActive { get; set; }

		/// <summary>
        /// Gets or sets the Is Administrator. 
        /// </summary>
        /// <value>The Is Administrator.</value>
		public Boolean? IsAdministrator { get; set; }

		/// <summary>
        /// Gets or sets the Skills. 
        /// </summary>
        /// <value>The Skills.</value>
		public String Skills { get; set; }
		
		/// <summary>
        /// Gets or sets the Owner ID. 
        /// </summary>
        /// <value>The Owner ID.</value>
		[ForeignKey]
		public Int32? ContactOwnerID { get; set; }
		

		
	}
	
	
	/// <summary>
    /// Contact fields
    /// </summary>
	public enum ContactField
	{
        /// <summary>
        /// First Name
        /// </summary>		
		FirstName,
        /// <summary>
        /// Last Name
        /// </summary>		
		LastName,
        /// <summary>
        /// Email
        /// </summary>		
		Email,
        /// <summary>
        /// Login
        /// </summary>		
		Login,
        /// <summary>
        /// Password
        /// </summary>		
		Password,
        /// <summary>
        /// Create Date
        /// </summary>		
		CreateDate,
        /// <summary>
        /// Modify Date
        /// </summary>		
		ModifyDate,
        /// <summary>
        /// Delete Date
        /// </summary>		
		DeleteDate,
        /// <summary>
        /// Is Active
        /// </summary>		
		IsActive,
        /// <summary>
        /// Is Administrator
        /// </summary>		
		IsAdministrator,
        /// <summary>
        /// Skills
        /// </summary>		
		Skills,
        /// <summary>
        /// Contact Owner ID
        /// </summary>		
		ContactOwnerID,
	}
}