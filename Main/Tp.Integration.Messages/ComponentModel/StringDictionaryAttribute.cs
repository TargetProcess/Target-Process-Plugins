// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
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