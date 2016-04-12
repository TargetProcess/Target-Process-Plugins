//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Tfs.Tests.Context;
using Tp.Tfs.StructureMap;

namespace Tp.Tfs.Tests.StructureMap
{
	public class VcsEnvironmentRegistry : TfsRegistry
	{
		public VcsEnvironmentRegistry()
		{
			For<VcsPluginContext>().HybridHttpOrThreadLocalScoped().Use<VcsPluginContext>();
            For<WorkItemsContext>().HybridHttpOrThreadLocalScoped().Use<WorkItemsContext>();
		}
	}
}