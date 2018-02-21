// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
    public class GetActivityLogCommand : IPluginCommand
    {
        private readonly IProfile _profile;

        public GetActivityLogCommand(IProfile profile)
        {
            _profile = profile;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            var filter = args.Deserialize<ActivityFilter>();
            var activity = _profile.Log.GetBy(filter);

            return new PluginCommandResponseMessage
            {
                ResponseData = activity.Serialize(),
                PluginCommandStatus = PluginCommandStatus.Succeed
            };
        }

        public string Name => EmbeddedPluginCommands.GetActivityLog;
    }
}
