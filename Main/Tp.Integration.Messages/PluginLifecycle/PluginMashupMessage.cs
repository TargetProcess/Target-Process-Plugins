// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable]
	public class PluginMashupMessage : IPluginLifecycleMessage
	{
		public PluginMashupScript[] PluginMashupScripts { get; set; }
		public PluginName PluginName { get; set; }
		public string[] Placeholders { get; set; }
		public string MashupName { get; set; }
	}

	[Serializable]
	public class PluginMashupScript
	{
		public string FileName { get; set; }
		public string ScriptContent { get; set; }
	}
}