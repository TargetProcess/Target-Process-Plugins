using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable]
	public class Mashup
	{
		public string MashupName { get; set; }
		public string[] MashupFilePaths { get; set; }
		public string[] MashupPhysicalFilePaths { get; set; }
		public MashupConfig MashupConfig { get; set; }
	}
}
