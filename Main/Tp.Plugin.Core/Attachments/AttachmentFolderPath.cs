// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;

namespace Tp.Plugin.Core.Attachments
{
	public class AttachmentFolderPath : IAttachmentFolderPath
	{
		private const string TEMP_ATTACHEMNTS_DIRECTORY_NAME = "TempAttachments";

		public string Value
		{
			get
			{
				var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMP_ATTACHEMNTS_DIRECTORY_NAME);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}
	}

	public interface IAttachmentFolderPath
	{
		string Value { get; }
	}
}