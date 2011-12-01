// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Plugin.Core.Attachments
{
	[Serializable]
	public class LocalStoredAttachment
	{
		public FileId FileId { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public int? OwnerId { get; set; }
		public DateTime? CreateDate { get; set; }
	}
}