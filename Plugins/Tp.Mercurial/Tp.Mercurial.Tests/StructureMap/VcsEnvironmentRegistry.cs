//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Mercurial.StructureMap;
using Tp.Mercurial.Tests.Context;

namespace Tp.Mercurial.Tests.StructureMap
{
	public class VcsEnvironmentRegistry : MercurialRegistry
	{
		public VcsEnvironmentRegistry()
		{
			For<VcsPluginContext>().HybridHttpOrThreadLocalScoped().Use<VcsPluginContext>();
		}
	}
}