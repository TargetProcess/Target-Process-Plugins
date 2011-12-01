//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Subversion.Context;

namespace Tp.Subversion.StructureMap
{
	public class VcsEnvironmentRegistry : SubversionRegistry
	{
		public VcsEnvironmentRegistry()
		{
			For<VcsPluginContext>().HybridHttpOrThreadLocalScoped().Use<VcsPluginContext>();
		}
	}
}