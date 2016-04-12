// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.Common.Mapping
{
	[DataContract]
	public class MappingElement
	{
		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public MappingLookup Value { get; set; }

		public bool IsEmptyValue()
		{
			return Value == null || (Value.Id == 0 && string.IsNullOrEmpty(Value.Name));
		}
	}
}