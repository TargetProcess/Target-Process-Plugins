// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using log4net;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.PopEmailIntegration.Sagas;

namespace Tp.PopEmailIntegration.Initialization
{
    [Serializable]
    public class ProfileServerAndLogin
    {
        public string MailServer { get; set; }
        public string Login { get; set; }
    }

    public class UserStorageMigrator : IWantToRunBeforeBusStart
    {
        private readonly ITpBus _bus;

        private readonly IAccountCollection _accountCollection;
        private readonly ILog _logger;

        public UserStorageMigrator(ILogManager logManager, IAccountCollection accountCollection)
        {
            var bus = ObjectFactory.GetInstance<ITpBus>();
            _bus = bus ?? throw new ArgumentNullException("bus");
            _accountCollection = accountCollection;
            _logger = logManager.GetLogger(GetType());
        }

        public void Run()
        {
            _logger.Info("Start user migration");

            _accountCollection.ForEach(a =>
            {
                a.Profiles.ForEach(p => MigrateProfile((IProfile)p, a, _logger));
            });

            _logger.Info("End user migration");
        }

        private void MigrateProfile(IProfile profile, IAccountReadonly account, ILog logger)
        {
            if (!profile.Initialized)
            {
                return;
            }
            var profileSettings = (ProjectEmailProfile)profile.Settings;
            if (profileSettings.UsersMigrated)
            {
                return;
            }
            profile.ToggleMessageHandling(true);
            logger.Info($"Migrate Users {account.Name}\\{profile.Name.Value}");
            _bus.SendLocalWithContext(profile.Name, account.Name, new MigrateUsersCommandInternal());
        }
    }

    internal static class ProfileHelper
    {
        /// <summary>
        /// Prevent handle of entity lifecycle messages from TP until indexing is finished
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="stopHandling"></param>
        internal static void ToggleMessageHandling(this IProfile profile, bool stopHandling)
        {
            if (stopHandling)
            {
                if (profile.Initialized)
                {
                    profile.MarkAsNotInitialized();
                    profile.Save();
                }
            }
            else
            {
                if (!profile.Initialized)
                {
                    profile.MarkAsInitialized();
                    profile.Save();
                }
            }
        }
    }
}
