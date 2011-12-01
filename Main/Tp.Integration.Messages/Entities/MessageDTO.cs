//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Message. Represents Message.
    /// </summary>
	[Serializable]
	public partial class MessageDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return MessageID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				MessageID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Message ID.
        /// </summary>
        /// <value>The Message ID.</value>
		[PrimaryKey]
		public int? MessageID { get; set; }
		

		/// <summary>
        /// Gets or sets the Subject. Subject of the Message
        /// </summary>
        /// <value>The Subject.</value>
		public String Subject { get; set; }

		/// <summary>
        /// Gets or sets the Recipients. Recipients
        /// </summary>
        /// <value>The Recipients.</value>
		public String Recipients { get; set; }

		/// <summary>
        /// Gets or sets the Is Read. Is Read
        /// </summary>
        /// <value>The Is Read.</value>
		public Boolean? IsRead { get; set; }

		/// <summary>
        /// Gets or sets the Is Processed. Is Processed
        /// </summary>
        /// <value>The Is Processed.</value>
		public Boolean? IsProcessed { get; set; }

		/// <summary>
        /// Gets or sets the Body. Body of the Message
        /// </summary>
        /// <value>The Body.</value>
		public String Body { get; set; }

		/// <summary>
        /// Gets or sets the Create Date. Date when Message has been created
        /// </summary>
        /// <value>The Create Date.</value>
		public DateTime? CreateDate { get; set; }

		/// <summary>
        /// Gets or sets the Send Date. Send Date
        /// </summary>
        /// <value>The Send Date.</value>
		public DateTime? SendDate { get; set; }

		/// <summary>
        /// Gets or sets the Message Type. Message Type
        /// </summary>
        /// <value>The Message Type.</value>
		public MessageTypeEnum? MessageType { get; set; }

		/// <summary>
        /// Gets or sets the Content Type. Content Type
        /// </summary>
        /// <value>The Content Type.</value>
		public ContentTypeEnum? ContentType { get; set; }
		
		/// <summary>
        /// Gets or sets the From ID. From
        /// </summary>
        /// <value>The From ID.</value>
		[ForeignKey]
		public Int32? FromID { get; set; }
		
		/// <summary>
        /// Gets or sets the To ID. To
        /// </summary>
        /// <value>The To ID.</value>
		[ForeignKey]
		public Int32? ToID { get; set; }
		
		/// <summary>
        /// Gets or sets the Message Uid ID. Message Uid
        /// </summary>
        /// <value>The Message Uid ID.</value>
		[ForeignKey]
		public Int32? MessageUidID { get; set; }

		public MessageUidDTO MessageUidDto { get; set; }
	}


	/// <summary>
    /// Message fields
    /// </summary>
	public enum MessageField
	{
        /// <summary>
        /// Subject
        /// </summary>		
		Subject,
        /// <summary>
        /// Recipients
        /// </summary>		
		Recipients,
        /// <summary>
        /// Is Read
        /// </summary>		
		IsRead,
        /// <summary>
        /// Is Processed
        /// </summary>		
		IsProcessed,
        /// <summary>
        /// Body
        /// </summary>		
		Body,
        /// <summary>
        /// Create Date
        /// </summary>		
		CreateDate,
        /// <summary>
        /// Send Date
        /// </summary>		
		SendDate,
        /// <summary>
        /// Message Type
        /// </summary>		
		MessageType,
        /// <summary>
        /// Content Type
        /// </summary>		
		ContentType,
        /// <summary>
        /// From ID
        /// </summary>		
		FromID,
        /// <summary>
        /// To ID
        /// </summary>		
		ToID,
        /// <summary>
        /// Message Uid ID
        /// </summary>		
		MessageUidID,
	}
}