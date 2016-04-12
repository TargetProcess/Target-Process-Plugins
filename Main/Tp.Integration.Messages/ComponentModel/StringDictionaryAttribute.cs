using System;

namespace Tp.Integration.Messages.ComponentModel
{
	[Serializable, AttributeUsage(AttributeTargets.Property)]
	public class StringDictionaryAttribute : Attribute
	{
		public StringDictionaryAttribute()
		{
		}

		public StringDictionaryAttribute(string keyName, string valueName, string category, string description)
		{
			KeyName = keyName;
			ValueName = valueName;
			Category = category;
			Description = description;
		}

		public string KeyName { get; set; }
		public string ValueName { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
	}
}
