// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Common
{
	/// <summary>
	/// Data Transfer object of Attachment File. Represents reference to file or file content.
	/// </summary>
	[Serializable]
	public class AttachmentFileDTO : DataTransferObject
	{
		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		/// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return AttachmentFileID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				AttachmentFileID = value;
			}
		}

		/// <summary>
		/// Gets or sets the Attachment File ID.
		/// </summary>
		/// <value>The Attachment File ID.</value>
		[PrimaryKey]
		public int? AttachmentFileID { get; set; }


		/// <summary>
		/// Gets or sets the Unique File Name. The path to file in the system
		/// </summary>
		/// <value>The Unique File Name.</value>
		public String UniqueFileName { get; set; }

		/// <summary>
		/// Gets or sets the Buffer. The content of the file if the attachment is stored in database
		/// </summary>
		/// <value>The Buffer.</value>
		public Byte[] Buffer { get; set; }
	}


	/// <summary>
	/// Attachment File fields
	/// </summary>
	public enum AttachmentFileField
	{
		/// <summary>
		/// Unique File Name
		/// </summary>		
		UniqueFileName,
		/// <summary>
		/// Buffer
		/// </summary>		
		Buffer,
	}
}