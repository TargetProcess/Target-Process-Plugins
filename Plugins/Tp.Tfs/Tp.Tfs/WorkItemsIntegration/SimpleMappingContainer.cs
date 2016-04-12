// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.Tfs.WorkItemsIntegration
{
	public class SimpleMappingContainer : List<SimpleMappingElement>
	{
		public string this[string first]
		{
			get
			{
				var mappingElement = Find(x => x.First.ToLower().Trim() == first.ToLower().Trim());
				return mappingElement == null ? null : mappingElement.Second;
			}
		}
	}
}
