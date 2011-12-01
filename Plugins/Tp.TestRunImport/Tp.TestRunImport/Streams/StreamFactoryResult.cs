// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.IO;

namespace Tp.Integration.Plugin.TestRunImport.Streams
{
	public class StreamFactoryResult
	{
		public Stream Stream { get; set; }
		public LastModifyResult LastModifyResult { get; set; }
	}

	public class LastModifyResult
	{
		public LastModifyResult()
		{
			ModifyTimeUtcsTicks = DateTime.MinValue.Ticks;
			ETagHeader = string.Empty;
		}

		public long ModifyTimeUtcsTicks { get; set; }
		public string ETagHeader { get; set; }
	}
}