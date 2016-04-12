// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Integration.Plugin.Common.PluginCommand
{
	public interface IPluginCommandRepository : IEnumerable<IPluginCommand>
	{
	}
}