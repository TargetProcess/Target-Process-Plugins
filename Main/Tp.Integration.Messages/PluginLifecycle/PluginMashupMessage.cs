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
		public AccountName AccountName { get; set; }
	}

	[Serializable]
	public class PluginMashupScript
	{
		public string FileName { get; set; }
		public string ScriptContent { get; set; }
	}
}
