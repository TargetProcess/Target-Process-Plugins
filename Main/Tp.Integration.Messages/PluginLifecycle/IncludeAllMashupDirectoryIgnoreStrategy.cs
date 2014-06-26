// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Messages.PluginLifecycle
{
	public class IncludeAllMashupDirectoryIgnoreStrategy : IMashupDirectoryIgnoreStrategy
	{
		public bool ShouldIgnoreMashupDirectory(string directory)
		{
			return false;
		}
	}
}