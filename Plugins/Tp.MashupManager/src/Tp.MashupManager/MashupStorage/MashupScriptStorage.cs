// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

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
			_log.Info(string.Format("Getting mashup with name '{0}'", mashupName));
			Mashup mashup = _mashupLoader.Load(GetMashupFolderPath(account, mashupName), mashupName);			
			_log.Info(string.Format("Mashup with name '{0}' retrieved", mashupName));
			return mashup;
		}

		public void SaveMashup(Mashup mashup)
		{
			_log.Info(string.Format("Saving mashup with name '{0}'", mashup.Name));
			var mashupFolderPath = GetMashupFolderPath(mashup.Name);
			EnsureMashupFolderExistsAndEmpty(mashupFolderPath);
			foreach (var file in mashup.Files)
			{
				EnsureMashupFileSubFolderExists(mashupFolderPath, file.FileName);
				File.WriteAllText(Path.Combine(mashupFolderPath, file.FileName), file.Content);
			}
			WritePlaceholdersFile(mashup, mashupFolderPath);
			WriteAccountsFile(mashup, mashupFolderPath);
			_log.Info(string.Format("Mashup with name '{0}' saved", mashup.Name));
		}

		public void DeleteMashup(string mashupName)
		{
			_log.Info(string.Format("Deleting mashup '{0}'", mashupName));

			var mashupFolderPath = GetMashupFolderPath(mashupName);
			Directory.Delete(mashupFolderPath, true);

			_log.Info(string.Format("Mashup '{0}' deleted", mashupName));
		}

		private void WriteAccountsFile(Mashup mashup, string mashupFolderPath)
		{
			if (_accountName.Value != AccountName.Empty)
			{
				_log.Info(string.Format("Add account config to mashup with name '{0}'", mashup.Name));

				var cfgPath = Path.Combine(mashupFolderPath, Mashup.AccountCfgFileName);
				File.WriteAllText(cfgPath, string.Format("{0}{1}", MashupConfig.AccountsConfigPrefix, _accountName.Value));
			}
		}

		private void WritePlaceholdersFile(Mashup mashup, string mashupFolderPath)
		{
			if (!string.IsNullOrEmpty(mashup.Placeholders))
			{
				_log.Info(string.Format("Add placeholder config to mashup with name '{0}'", mashup.Name));

				var cfgPath = Path.Combine(mashupFolderPath, "placeholders.cfg");
				File.WriteAllText(cfgPath, string.Format("{0}{1}", MashupConfig.PlaceholderConfigPrefix, mashup.Placeholders));
			}
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
			var nameWithAccount =
				string.Format("{0} {1}", accountName.Value != AccountName.Empty ? accountName.Value : string.Empty, mashupName).
					Trim();
			var mashupFolderPath = Path.Combine(_folder.Path, nameWithAccount);

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