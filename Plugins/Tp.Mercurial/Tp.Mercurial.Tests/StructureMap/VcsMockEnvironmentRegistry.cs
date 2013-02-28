//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Mercurial.Tests.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.Subversion.StructureMap;

namespace Tp.Mercurial.Tests.StructureMap
{
	public class VcsMockEnvironmentRegistry : VcsEnvironmentRegistry
	{
		private readonly IVersionControlSystem _vcsMock = new VersionControlSystemMock();
		protected override void ConfigureVersionControlSystem()
		{
			For<IVersionControlSystem>().HybridHttpOrThreadLocalScoped().Use(_vcsMock);
			Forward<IVersionControlSystem, VersionControlSystemMock>();
		}
	}
}