//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Git.StructureMap;
using Tp.Git.Tests.Context;

namespace Tp.Git.Tests.StructureMap
{
	public class VcsEnvironmentRegistry : GitRegistry
	{
		public VcsEnvironmentRegistry()
		{
			For<VcsPluginContext>().HybridHttpOrThreadLocalScoped().Use<VcsPluginContext>();
		}
	}
}