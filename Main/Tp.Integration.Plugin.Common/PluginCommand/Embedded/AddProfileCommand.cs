//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
    public class AddProfileCommand : EditProfileCommandBase
    {
        public AddProfileCommand(ITpBus bus, IProfileCollection profileCollection, IPluginContext pluginContext,
            IPluginMetadata pluginMetadata)
            : base(profileCollection, bus, pluginContext, pluginMetadata)
        {
        }

        protected override PluginCommandResponseMessage OnExecute(PluginProfileDto profileDto)
        {
            NormalizeProfile(profileDto);

            AddPluginProfile(profileDto);
            SendProfileChangedLocalMessage(profileDto.Name, new ProfileAddedMessage());

            return new PluginCommandResponseMessage
                { ResponseData = string.Empty.Serialize(), PluginCommandStatus = PluginCommandStatus.Succeed };
        }

        public override string Name
        {
            get { return EmbeddedPluginCommands.AddProfile; }
        }
    }
}
