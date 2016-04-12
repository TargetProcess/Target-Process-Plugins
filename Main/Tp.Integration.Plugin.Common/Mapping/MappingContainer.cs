// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common.Mapping
{
	public class Mappings
	{
		public MappingContainer States { get; set; }

		public MappingContainer Severities { get; set; }

		public MappingContainer Priorities { get; set; }
	}

	public class MappingContainer : List<MappingElement>
	{
		public MappingLookup this[string key]
		{
			get
			{
				var mappingElement = Find(x => x.Key.ToLower().Trim() == key.ToLower().Trim());
				return mappingElement == null ? null : mappingElement.Value;
			}
		}
	}
}