using System;
using System.Runtime.Serialization;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Search.Bus
{
	[Profile, Serializable, DataContract]
	public class SearcherProfile : IValidatable, ISynchronizableProfile
	{
		public const string Name = "Now running";
		public SearcherProfile()
		{
			SynchronizationInterval = 5;
			EnabledForTp2 = false;
		}

		[DataMember]
		public int SynchronizationInterval { get; set; }

		[DataMember]
		public bool EnabledForTp2 { get; set; }

		public void Validate(PluginProfileErrorCollection errors)
		{
		}
	}
}
