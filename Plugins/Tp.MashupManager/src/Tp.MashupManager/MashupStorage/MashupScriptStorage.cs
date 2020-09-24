using System;
using System.IO;
using System.Linq;
using log4net;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.MashupManager.MashupStorage
{
    public class MashupScriptStorage : IMashupScriptStorage
    {
        private readonly IMashupLocalFolder _folder;
        private readonly IMashupLoader _mashupLoader;
        private readonly AccountName _accountName;
        private readonly ILog _log;

        public MashupScriptStorage(IPluginContext context, IMashupLocalFolder folder, ILogManager logManager, IMashupLoader mashupLoader)
        {
            _folder = folder;
            _mashupLoader = mashupLoader;
            _log = logManager.GetLogger(GetType());
            _accountName = context.AccountName;
        }

        public Mashup GetMashup(string mashupName)
        {
            return GetMashup(_accountName, mashupName);
        }

        public Mashup GetMashup(AccountName account, string mashupName)
        {
            _log.Info($"Retrieving mashup '{mashupName}'");
            Mashup mashup = _mashupLoader.Load(GetMashupFolderPath(account, mashupName), mashupName);
            _log.Info($"Mashup '{mashupName}' retrieved");
            return mashup;
        }

        public void SaveMashup(Mashup mashup)
        {
            _log.Info($"Saving mashup '{mashup.Name}'");

            var mashupFolderPath = GetMashupFolderPath(mashup.Name);
            EnsureMashupFolderExistsAndEmpty(mashupFolderPath);

            foreach (var file in mashup.Files)
            {
                var mashupFileFullPath = Path.Combine(mashupFolderPath, file.FileName);
                if (!new DirectoryInfo(mashupFolderPath).HasParentChildRelation(new FileInfo(mashupFileFullPath).Directory))
                {
                    throw new BadMashupFileNameException("Bad mashup file name {0}".Fmt(file.FileName));
                }
                EnsureMashupFileSubFolderExists(mashupFolderPath, file.FileName);
                File.WriteAllText(mashupFileFullPath, file.Content);
            }

            WriteMashupConfig(mashup, mashupFolderPath);
            WriteAccountsConfig(mashup.Name, mashupFolderPath);

            _log.Info($"Mashup '{mashup.Name}' saved");
        }

        public void DeleteMashup(string mashupName)
        {
            _log.Info($"Deleting mashup '{mashupName}'");

            var mashupFolderPath = GetMashupFolderPath(mashupName);
            if (Directory.Exists(mashupFolderPath))
            {
                Directory.Delete(mashupFolderPath, true);
                _log.Info($"Mashup '{mashupName}' deleted");
            }
            else
            {
                _log.Warn($"Mashup '{mashupName}' doesn't exist in mashups folder");
            }
        }

        private void WriteAccountsConfig(string mashupName, string mashupFolderPath)
        {
            if (_accountName.Value != AccountName.Empty)
            {
                _log.Info($"Add account config to mashup '{mashupName}'");

                var cfgPath = Path.Combine(mashupFolderPath, Mashup.AccountCfgFileName);
                File.WriteAllText(cfgPath, MashupConfig.AccountsConfigLine(_accountName));
            }
        }

        private void WriteMashupConfig(Mashup mashup, string mashupFolderPath)
        {
            _log.Info($"Add placeholder config to mashup '{mashup.Name}'");
            var lines = MashupConfig.GetConfigLines(mashup.MashupMetaInfo, mashup.Placeholders);
            var cfgPath = Path.Combine(mashupFolderPath, "placeholders.cfg");
            File.WriteAllText(cfgPath, lines.ToString(Environment.NewLine));
        }

        private static void EnsureMashupFolderExistsAndEmpty(string mashupFolderPath)
        {
            if (!Directory.Exists(mashupFolderPath))
            {
                Directory.CreateDirectory(mashupFolderPath);
            }
            else
            {
                Directory.GetFiles(mashupFolderPath, "*", SearchOption.AllDirectories).ForEach(File.Delete);
                Directory.GetDirectories(mashupFolderPath, "*", SearchOption.TopDirectoryOnly).ForEach(d => Directory.Delete(d, true));
            }
        }

        private string GetMashupFolderPath(AccountName accountName, string mashupName)
        {
            var nameWithAccount = $"{(accountName.Value == AccountName.Empty ? string.Empty : accountName.Value)} {mashupName}".Trim();
            var mashupFolderPath = Path.Combine(_folder.Path, nameWithAccount);
            if (!new DirectoryInfo(_folder.Path).HasParentChildRelation(new DirectoryInfo(mashupFolderPath)))
            {
                throw new BadMashupNameException("Bad mashup name {0}".Fmt(mashupName));
            }
            return mashupFolderPath;
        }

        private string GetMashupFolderPath(string mashupName)
        {
            return GetMashupFolderPath(_accountName, mashupName);
        }

        private static void EnsureMashupFileSubFolderExists(string mashupFolderPath, string mashupFilePath)
        {
            var subFolderRelativePath = Path.GetDirectoryName(mashupFilePath);

            if (!string.IsNullOrEmpty(subFolderRelativePath))
            {
                var subFolderFullPath = Path.Combine(mashupFolderPath, subFolderRelativePath);

                if (!Directory.Exists(subFolderFullPath))
                {
                    Directory.CreateDirectory(subFolderFullPath);
                }
            }
        }
    }
}
