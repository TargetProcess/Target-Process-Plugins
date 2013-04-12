// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Xml.Serialization;using System.Runtime.Serialization;

namespace Tp.Integration.Common
{
	/// <summary>
	/// Data Transfer object of Message Uid. Represents reference to downloaded message.
	/// </summary>
	[Serializable][DataContract]
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
		[DataMember][XmlElement(Order = 3)]public int? MessageUidID { get; set; }


		/// <summary>
		/// Gets or sets the UID. The identity of the message
		/// </summary>
		/// <value>The UID.</value>
		[DataMember][XmlElement(Order = 4)]public String UID { get; set; }

		/// <summary>
		/// Gets or sets the Mail Server. Mail server name
		/// </summary>
		/// <value>The Mail Server.</value>
		[DataMember][XmlElement(Order = 5)]public String MailServer { get; set; }

		/// <summary>
		/// Gets or sets the Mail Login. Mail login
		/// </summary>
		/// <value>The Mail Login.</value>
		[DataMember][XmlElement(Order = 6)]public String MailLogin { get; set; }
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
