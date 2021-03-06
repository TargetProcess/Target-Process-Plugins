﻿//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader.Client;

namespace Tp.PopEmailIntegration.CustomCommands
{
    public class CheckConnectionCommand : IPluginCommand
    {
        private readonly IProfileCollection _profileCollection;

        public CheckConnectionCommand(IProfileCollection profileCollection)
        {
            _profileCollection = profileCollection;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            return new PluginCommandResponseMessage
                { PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = GetResponse(args) };
        }

        private string GetResponse(string args)
        {
            var profile = args.DeserializeProfile(p => _profileCollection[p]);
            var settings = (ConnectionSettings) profile.Settings;
            var errors = new PluginProfileErrorCollection();

            settings.ValidateConnection(errors);
            if (errors.Any())
            {
                return errors.Serialize();
            }

            CheckConnection(settings, errors);
            return errors.Any() ? errors.Serialize() : string.Empty;
        }

        private static void CheckConnection(ConnectionSettings settings, PluginProfileErrorCollection errors)
        {
            using (IEmailClient client = new MailBeeEmailClient(settings))
            {
                client.CheckConnection(errors);
                client.Disconnect();
            }
        }

        public string Name { get; } = "CheckConnection";
    }
}
