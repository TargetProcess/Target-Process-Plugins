// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
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