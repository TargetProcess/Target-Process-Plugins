﻿// 
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.PopEmailIntegration.Initialization
{
    public class ProfileAddedMessageHandler : IHandleMessages<ProfileAddedMessage>
    {
        private readonly IStorageRepository _storageRepository;

        public ProfileAddedMessageHandler(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }

        public void Handle(ProfileAddedMessage message)
        {
            var profile = _storageRepository.GetProfile<ProjectEmailProfile>();
            var profileServerAndLogins = _storageRepository.Get<ProfileServerAndLogin>();
            var login = profile.SecureAccessMethod == SecureAccessMethods.LoginAndPassword ? profile.Login : profile.OAuthState.Email;
            profileServerAndLogins.Add(new ProfileServerAndLogin { MailServer = profile.MailServer, Login = login });
        }
    }
}
