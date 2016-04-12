using System;

namespace Tp.Integration.Messages.ComponentModel
{
	[Serializable, AttributeUsage(AttributeTargets.Property)]
	public class TPDescriptionAttribute : Attribute
	{
		public TPDescriptionAttribute()
		{
		}

		public TPDescriptionAttribute(string value)
		{
			Value = value;
		}

		public string Value { get; set; }
	}
}
