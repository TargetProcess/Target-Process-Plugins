// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Integration.Plugin.Common.PluginCommand
{
	public class PluginCommandRepository : IPluginCommandRepository
	{
		private readonly List<Type> _pluginCommandTypes;
		private readonly object _gate;

		public PluginCommandRepository()
		{
			_pluginCommandTypes = Configure.TypesToScan.Where(type => !type.IsAbstract && typeof(IPluginCommand).IsAssignableFrom(type)).Distinct().ToList();
			_gate = new object();
		}
		
		public IEnumerator<IPluginCommand> GetEnumerator()
		{
			lock (_gate)
			{
				return _pluginCommandTypes.Select(cmdType => ObjectFactory.GetInstance(cmdType) as IPluginCommand).ToList().GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}