// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;

namespace Tp.Tfs.WorkItemsIntegration.Exstentions
{
	public static class EntityMappingExstentions
	{
		public static string ToWorkItemsString(this SimpleMappingContainer entityMapping)
		{
			string sequence = entityMapping.Select(x => string.Format("'{0}'", x.First)).ToString(",");
			return sequence;
		}
	}
}
