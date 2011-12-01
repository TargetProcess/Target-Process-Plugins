// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections;
using System.Collections.Generic;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand;

namespace Tp.Integration.Plugin.Common.Tests.Common.PluginCommand
{
	public class PluginCommandMockRepository : IPluginCommandRepository
	{
		private readonly List<IPluginCommand> _commands = new List<IPluginCommand>();

		public void Add(IPluginCommand pluginCommand)
		{
			_commands.Add(pluginCommand);
		}

		public IEnumerator<IPluginCommand> GetEnumerator()
		{
			return _commands.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}