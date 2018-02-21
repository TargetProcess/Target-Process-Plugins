// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Integration.Plugin.Common.Handlers
{
    public class ProfileUpdatedHandler : IHandleMessages<ProfileUpdatedMessage>
    {
        private readonly IProfile _profile;

        public ProfileUpdatedHandler(IProfile profile)
        {
            _profile = profile;
        }

        public void Handle(ProfileUpdatedMessage message)
        {
            _profile.Log.ClearBy(new ActivityFilter { Type = ActivityType.Errors });
        }
    }
}
