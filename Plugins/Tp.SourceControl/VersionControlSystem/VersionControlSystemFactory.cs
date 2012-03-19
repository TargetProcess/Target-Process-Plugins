// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using StructureMap.Pipeline;
using Tp.SourceControl.Settings;

namespace Tp.SourceControl.VersionControlSystem
{
	public class VersionControlSystemFactory : IVersionControlSystemFactory
	{
		public IVersionControlSystem Get(ISourceControlConnectionSettingsSource settings)
		{
			var vcsArgs = new ExplicitArguments();
			vcsArgs.Set(settings);
			return ObjectFactory.GetInstance<IVersionControlSystem>(vcsArgs);
		}
	}
}