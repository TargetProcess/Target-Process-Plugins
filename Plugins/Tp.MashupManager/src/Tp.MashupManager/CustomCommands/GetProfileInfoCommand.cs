// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.MashupManager.CustomCommands
{
    public class GetProfileInfoCommand : IPluginCommand
    {
        public PluginCommandResponseMessage Execute(string args, UserDTO user = null)
        {
            IProfile profile = ObjectFactory.GetInstance<ISingleProfile>().Profile;

            return profile == null ? GetEmptyResponseMessage() : GetResponseMessage(profile);
        }

        public string Name => "GetProfileInfo";

        private PluginCommandResponseMessage GetResponseMessage(IProfile profile)
        {
            var command = ObjectFactory.GetInstance<GetProfileCommand>();
            return command.Execute(profile.Name.Value, null);
        }

        private PluginCommandResponseMessage GetEmptyResponseMessage()
        {
            return new PluginCommandResponseMessage
            {
                PluginCommandStatus = PluginCommandStatus.Succeed,
                ResponseData = new PluginProfileDto { Name = string.Empty, Settings = new MashupManagerProfile() }.SerializeForClient()
            };
        }
    }
}
