using System;
using System.Runtime.Serialization;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus
{
	[Profile, Serializable, DataContract]
	public class SearcherProfile : IValidatable, ISynchronizableProfile
	{
		public const string Name = "Now running";
		public SearcherProfile()
		{
			EnabledForTp2 = false;
		}

		[DataMember]
		public int SynchronizationInterval
		{
			get { return ObjectFactory.GetInstance<DocumentIndexSetup>().CheckIntervalInMinutes; }
			set { }
		}

		[DataMember]
		public bool EnabledForTp2 { get; set; }

		public void Validate(PluginProfileErrorCollection errors)
		{
		}
	}
}
