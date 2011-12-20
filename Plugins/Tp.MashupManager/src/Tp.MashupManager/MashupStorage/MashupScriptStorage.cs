// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

namespace Tp.MashupManager.MashupStorage
{
	public class MashupScriptStorage : IMashupScriptStorage
	{
		private readonly IMashupLocalFolder _folder;
		private readonly AccountName _accountName;
		private ILog _log;

		public MashupScriptStorage(IPluginContext context, IMashupLocalFolder folder, ILogManager logManager)
		{
			_folder = folder;
			_log = logManager.GetLogger(GetType());
			_accountName = context.AccountName;
		}

		public MashupDto GetMashup(string mashupName)
		{
			_log.Info(string.Format("Getting mashup with name '{0}'", mashupName));

			string mashupFolderPath = GetMashupFolderPath(mashupName);

			if(!Directory.Exists(mashupFolderPath))
			{
				return null;
			}

			var script = Directory.GetFiles(mashupFolderPath, "*.js");
			var config = Directory.GetFiles(mashupFolderPath, MashupDto.PlaceholdersCfgFileName);

			if(!script.Any())
			{
				return null;
			}

			string scriptFileName = script.First();
			var scriptFileInfo = new FileInfo(scriptFileName);
			var mashup = new MashupDto
			             	{
			             		Name = scriptFileInfo.Name.Replace(scriptFileInfo.Extension, string.Empty),
								Placeholders = config.Any() ? File.ReadAllText(config.First()).Replace(MashupConfig.PlaceholderConfigPrefix, string.Empty) : null,
								Script = File.ReadAllText(scriptFileName)
			             	};
			_log.Info(string.Format("Mashup with name '{0}' retrieved", mashupName));

			return mashup;
		}

		public void SaveMashup(MashupDto mashup)
		{
			_log.Info(string.Format("Saving mashup with name '{0}'", mashup.Name));

			var mashupFolderPath = GetMashupFolderPath(mashup.Name);
			if (!Directory.Exists(mashupFolderPath))
			{
				Directory.CreateDirectory(mashupFolderPath);
			}
			else
			{
				foreach (var file in Directory.GetFiles(mashupFolderPath, "*.*", SearchOption.AllDirectories))
				{
					File.Delete(file);
				}
			}

			var scriptPath = Path.Combine(mashupFolderPath, mashup.Name + ".js");
			File.WriteAllText(scriptPath, mashup.Script);

			if (!string.IsNullOrEmpty(mashup.Placeholders))
			{
				_log.Info(string.Format("Add placeholder config to mashup with name '{0}'", mashup.Name));

				var cfgPath = Path.Combine(mashupFolderPath, MashupDto.PlaceholdersCfgFileName);
				File.WriteAllText(cfgPath, string.Format("{0}{1}", MashupConfig.PlaceholderConfigPrefix, mashup.Placeholders));
			}

			if(_accountName.Value != AccountName.Empty)
			{
				_log.Info(string.Format("Add account config to mashup with name '{0}'", mashup.Name));

				var cfgPath = Path.Combine(mashupFolderPath, MashupDto.AccountCfgFileName);
				File.WriteAllText(cfgPath, string.Format("{0}{1}", MashupConfig.AccountsConfigPrefix, _accountName.Value));
			}

			_log.Info(string.Format("Mashup with name '{0}' saved", mashup.Name));
		}

		public void DeleteMashup(string mashupName)
		{
			_log.Info(string.Format("Deleting mashup '{0}'", mashupName));

			var mashupFolderPath = GetMashupFolderPath(mashupName);
			Directory.Delete(mashupFolderPath, true);

			_log.Info(string.Format("Mashup '{0}' deleted", mashupName));
		}

		private string GetMashupFolderPath(string mashupName)
		{
			var nameWithAccount =
				string.Format("{0} {1}", _accountName.Value != AccountName.Empty ? _accountName.Value : string.Empty, mashupName).
					Trim();
			var mashupFolderPath = Path.Combine(_folder.Path, nameWithAccount);

			return mashupFolderPath;
		}
	}
}