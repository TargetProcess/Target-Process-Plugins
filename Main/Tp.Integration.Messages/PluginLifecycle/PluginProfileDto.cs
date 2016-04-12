using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable, DataContract]
	public class PluginProfileTypedDto<T> : PluginProfileDto
		where T : class, new()
	{
		[DataMember(Name = "Settings")]
		public T TypedSettings { get; set; }

		public override object Settings
		{
			get { return TypedSettings; }
			set { TypedSettings = value as T ?? new T(); }
		}

		public PluginProfileTypedDto()
		{
			TypedSettings = new T();
		}
	}

	[Serializable, DataContract]
	public class PluginProfileDto
	{
		[DataMember]
		public string Name { get; set; }

		public virtual object Settings { get; set; }

		public PluginProfileDto()
		{
			Settings = new object();
		}
	}
}
