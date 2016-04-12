using System;

namespace Tp.Integration.Messages.ComponentModel
{
	[Serializable, AttributeUsage(AttributeTargets.Property)]
	public class TpCategotyAttribute : Attribute
	{
		public TpCategotyAttribute()
		{
		}

		public TpCategotyAttribute(string value)
		{
			Value = value;
		}

		public string Value { get; set; }
	}
}
