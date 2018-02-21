using System.Linq;
using log4net;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Plugins.Toolkit.Repositories;
using Tp.SourceControl.Settings;

namespace Tp.SourceControl
{
    public class UserStorageMigrator : IWantToRunBeforeBusStart
    {
        private readonly IAccountCollection _accountCollection;
        private readonly ILog _logger;

        public UserStorageMigrator(ILogManager logManager, IAccountCollection accountCollection)
        {
            _accountCollection = accountCollection;
            _logger = logManager.GetLogger(GetType());
        }

        public void Run()
        {
            _logger.Info("Start user migration");

            _accountCollection.ForEach(a =>
            {
                a.Profiles.ForEach(p => MigrateProfile((IProfile) p, a, _logger));
            });

            _logger.Info("End user migration");
        }

        private static void MigrateProfile(IProfile profile, IAccountReadonly account, ILog logger)
        {
            var profileSettings = (SourceControlSettings) profile.Settings;
            if (profileSettings.UsersMigrated)
            {
                return;
            }

            logger.Info($"Start migrate {account.Name}\\{profile.Name.Value}");
            var users = profile.Get<UserDTO>();
            if (!users.Empty())
            {               
                var repository = new DataRepository<TpUserData>(profile);

                // in case previous migration has failed
                repository.GetAll().ForEach(repository.Delete);

                users.ForEach(u => repository.Add(new TpUserData(u)));
                users.Clear();                
            }

            profileSettings.UsersMigrated = true;
            profile.Save();

            logger.Info($"End migrate {account.Name}\\{profile.Name.Value}");
        }
    }
}