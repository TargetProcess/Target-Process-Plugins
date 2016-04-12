// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using log4net;

namespace Tp.Integration.Plugin.Common.PluginCommand
{
	public class PluginCommandHandler : IHandleMessages<ExecutePluginCommandCommand>
	{
		private readonly ITpBus _tpBus;
		private readonly IPluginCommandRepository _pluginCommandRepository;
		private readonly ILog _log;

		public PluginCommandHandler(ITpBus tpBus, IPluginCommandRepository pluginCommandRepository, ILogManager logManager)
		{
			_tpBus = tpBus;
			_pluginCommandRepository = pluginCommandRepository;
			_log = logManager.GetLogger(GetType());
		}

		public void Handle(ExecutePluginCommandCommand message)
		{
			_log.Info("Executing plugin command : {0}".Fmt(message.CommandName));
			try
			{
				var replyMessage = new PluginCommandResponseMessage();
				var commandsToExecute = _pluginCommandRepository.Where(x => x.Name == message.CommandName).ToArray();

				if (commandsToExecute.Count() > 1)
				{
					replyMessage.ResponseData = string.Format("There are more than one command with name '{0}'", message.CommandName);
					replyMessage.PluginCommandStatus = PluginCommandStatus.Error;
				}
				else if (!commandsToExecute.Any())
				{
					replyMessage.ResponseData = string.Format("No command with name '{0}' was found", message.CommandName);
					replyMessage.PluginCommandStatus = PluginCommandStatus.Error;
				}
				else
				{
					replyMessage = commandsToExecute[0].Execute(message.Arguments, message.User);
				}

				_tpBus.Reply(replyMessage);
				_log.Info("plugin command executed: {0}".Fmt(message.CommandName));
			}
			catch (PluginProfileValidationException validationException)
			{
				_log.Info("Profile validation failed during executing command {0} : {1}".Fmt(message.CommandName, validationException.Errors.Serialize()));
				_tpBus.Reply(new PluginCommandResponseMessage
				             	{
				             		ResponseData = validationException.Errors.Serialize(),
				             		PluginCommandStatus = PluginCommandStatus.Fail
				             	});
			}
			catch (Exception e)
			{
				_log.Error(string.Format("Plugin {0} command processing failed.", message.CommandName), e);
				_tpBus.Reply(new PluginCommandResponseMessage
				             	{
				             		ResponseData =
				             			string.Format("Plugin {0} command processing error: {1}", message.CommandName, e.Message),
				             		PluginCommandStatus = PluginCommandStatus.Error
				             	});
			}
		}
	}
}