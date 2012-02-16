// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager.CustomCommands
{
	public abstract class CrudMashupCommand<T> : IPluginCommand
		where T : class
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			var mashup = args.Deserialize<T>();
			var errors = ExecuteOperation(mashup);

			return new PluginCommandResponseMessage
			       	{
			       		PluginCommandStatus = errors.Any()
			       		                      	? PluginCommandStatus.Fail
			       		                      	: PluginCommandStatus.Succeed,
			       		ResponseData = errors.Serialize()
			       	};
		}

		protected abstract PluginProfileErrorCollection ExecuteOperation(T mashup);

		public abstract string Name { get; }

		protected static IMashupInfoRepository MashupRepository
		{
			get { return ObjectFactory.GetInstance<IMashupInfoRepository>(); }
		}
	}
}