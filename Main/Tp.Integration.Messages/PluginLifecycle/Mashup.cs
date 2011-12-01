// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable]
	public class Mashup
	{
		public string MashupName { get; set; }
		public string[] MashupFilePaths { get; set; }
		public MashupConfig MashupConfig { get; set; }
	}
}