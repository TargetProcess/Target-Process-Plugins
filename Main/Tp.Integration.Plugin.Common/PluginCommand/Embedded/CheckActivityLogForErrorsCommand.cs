// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
    public class CheckActivityLogForErrorsCommand : IPluginCommand
    {
        private readonly IProfileCollection _profileCollection;

        public CheckActivityLogForErrorsCommand(IProfileCollection profileCollection)
        {
            _profileCollection = profileCollection;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            var responseData =
                _profileCollection.Select(
                        x => new ProfileErrorCheckResult { ProfileName = x.Name.Value, ErrorsExist = x.Log.CheckForErrors() }).ToArray().
                    Serialize();
            return new PluginCommandResponseMessage
                { ResponseData = responseData, PluginCommandStatus = PluginCommandStatus.Succeed };
        }

        public string Name
        {
            get { return EmbeddedPluginCommands.CheckActivityLogForErrors; }
        }
    }
}
