// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Common
{
	/// <summary>
	/// Data Transfer object of Message Uid. Represents reference to downloaded message.
	/// </summary>
	[Serializable]
	public class MessageUidDTO : DataTransferObject
	{
		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		/// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return MessageUidID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				MessageUidID = value;
			}
		}

		/// <summary>
		/// Gets or sets the Message Uid ID.
		/// </summary>
		/// <value>The Message Uid ID.</value>
		[PrimaryKey]
		public int? MessageUidID { get; set; }


		/// <summary>
		/// Gets or sets the UID. The identity of the message
		/// </summary>
		/// <value>The UID.</value>
		public String UID { get; set; }

		/// <summary>
		/// Gets or sets the Mail Server. Mail server name
		/// </summary>
		/// <value>The Mail Server.</value>
		public String MailServer { get; set; }

		/// <summary>
		/// Gets or sets the Mail Login. Mail login
		/// </summary>
		/// <value>The Mail Login.</value>
		public String MailLogin { get; set; }
	}


	/// <summary>
	/// Message Uid fields
	/// </summary>
	public enum MessageUidField
	{
		/// <summary>
		/// UID
		/// </summary>		
		UID,
		/// <summary>
		/// Mail Server
		/// </summary>		
		MailServer,
		/// <summary>
		/// Mail Login
		/// </summary>		
		MailLogin,
	}
}