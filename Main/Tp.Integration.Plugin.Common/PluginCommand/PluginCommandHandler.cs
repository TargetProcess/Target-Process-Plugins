﻿// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using log4net;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand
{
    public class PluginCommandHandler : IHandleMessages<ExecutePluginCommandCommand>
    {
        private readonly ITpBus _tpBus;
        private readonly IPluginCommandRepository _pluginCommandRepository;
        private readonly IPluginContext _pluginContext;
        private readonly IDisabledAccountCollection _disabledAccountCollection;
        private readonly ILog _log;

        public PluginCommandHandler(ITpBus tpBus, IPluginCommandRepository pluginCommandRepository, ILogManager logManager,
            IPluginContext pluginContext, IDisabledAccountCollection disabledAccountCollection)
        {
            _tpBus = tpBus;
            _pluginCommandRepository = pluginCommandRepository;
            _pluginContext = pluginContext;
            _disabledAccountCollection = disabledAccountCollection;
            _log = logManager.GetLogger(GetType());
        }

        public void Handle(ExecutePluginCommandCommand message)
        {
            _log.Info("Executing plugin command : {0}".Fmt(message.CommandName));
            try
            {
                if (_disabledAccountCollection.Contains(_pluginContext.AccountName))
                {
                    throw new PluginDisabledForAccountException(_pluginContext.PluginName, _pluginContext.AccountName);
                }

                var replyMessage = new PluginCommandResponseMessage();
                var commandsToExecute = _pluginCommandRepository.Where(x => x.Name == message.CommandName).ToArray();

                if (commandsToExecute.Length > 1)
                {
                    replyMessage.ResponseData = $"There are more than one command with name '{message.CommandName}'";
                    replyMessage.PluginCommandStatus = PluginCommandStatus.Error;
                }
                else if (!commandsToExecute.Any())
                {
                    replyMessage.ResponseData = $"No command with name '{message.CommandName}' was found";
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
                _log.Info("Profile validation failed during executing command {0} : {1}".Fmt(message.CommandName,
                    validationException.Errors.Serialize()));
                _tpBus.Reply(new PluginCommandResponseMessage
                {
                    ResponseData = validationException.Errors.Serialize(),
                    PluginCommandStatus = PluginCommandStatus.Fail
                });
            }
            catch (Exception e)
            {
                _log.Error($"Plugin {message.CommandName} command processing failed.", e);
                _tpBus.Reply(new PluginCommandResponseMessage
                {
                    ResponseData =
                        $"Plugin {message.CommandName} command processing error: {e.Message}",
                    PluginCommandStatus = PluginCommandStatus.Error
                });
            }
        }
    }
}
