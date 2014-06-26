// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.MashupStorage;
using log4net;

namespace Tp.MashupManager
{
	public class MashupInfoRepository : IMashupInfoRepository
	{
		private readonly ITpBus _bus;
		private readonly IMashupScriptStorage _scriptStorage;
		private readonly IPluginContext _context;
		private readonly Lazy<MashupManagerProfile> _lazyMashupManagerProfile;
		private readonly ILog _log;
		public const string ProfileName = "Mashups";

		public MashupInfoRepository(ILogManager logManager, IPluginContext context, ITpBus bus, IMashupScriptStorage scriptStorage)
		{
			_bus = bus;
			_scriptStorage = scriptStorage;
			_context = context;
			_lazyMashupManagerProfile =
				Lazy.Create(() => ObjectFactory.GetInstance<ISingleProfile>().Profile.GetProfile<MashupManagerProfile>());
			_log = logManager.GetLogger(GetType());
		}

		public PluginProfileErrorCollection Add(Mashup dto, bool generateUniqueName)
		{
			if (generateUniqueName)
			{
				dto.Name = GetUniqueMashupName(dto.Name);
			}

			var errors = dto.ValidateAdd(ManagerProfile);
			if (errors.Any())
			{
				return errors;
			}

			AddMashupNameToPlugin(dto.Name);
			_scriptStorage.SaveMashup(dto);
			_bus.Send(dto.CreatePluginMashupMessage());
			_log.InfoFormat("Add mashup command sent to TP (Mashup '{0}' for account '{1}')", dto.Name, _context.AccountName.Value);
			return errors;
		}

		public PluginProfileErrorCollection Update(UpdateMashupCommandArg commandArg)
		{
			if (commandArg.IsNameChanged())
			{
				CompleteWithNotChangedOriginFiles(commandArg);
				var addErrors = Add(commandArg, false);

				if (!addErrors.Any())
				{
					var deleteErrors = Delete(commandArg.OldName);
					foreach (var error in deleteErrors)
					{
						addErrors.Add(error);
					}
				}

				return addErrors;
			}

			return UpdateInternal(commandArg);
		}

		public PluginProfileErrorCollection Delete(string name)
		{
			DeleteMashupNameToPlugin(name);
			_scriptStorage.DeleteMashup(name);
			var dto = new Mashup {Name = name};
			_bus.Send(dto.CreatePluginMashupMessage());
			_log.InfoFormat("Clean mashup commnad sent to TP (Mashup '{0}' for account '{1}')", dto.Name, _context.AccountName.Value);
			return new PluginProfileErrorCollection();
		}

		private string GetUniqueMashupName(string mashupName)
		{
			var uniqueName = mashupName;
			var index = 1;

			while (ManagerProfile.MashupNames.Any(n => n.EqualsIgnoreCase(uniqueName)))
			{
				uniqueName = "{0} {1}".Fmt(mashupName, index);
				index++;
			}
			
			return uniqueName;
		}

		private MashupManagerProfile ManagerProfile
		{
			get { return _lazyMashupManagerProfile.Value; }
		}		

		private void AddMashupNameToPlugin(string name)
		{
			ManagerProfile.MashupNames.Add(name);
			UpdateProfile(ManagerProfile);
		}

		private void DeleteMashupNameToPlugin(string name)
		{
			ManagerProfile.MashupNames.Remove(name);
			UpdateProfile(ManagerProfile);
		}

		private PluginProfileErrorCollection UpdateInternal(UpdateMashupCommandArg commandArg)
		{
			var errors = commandArg.ValidateUpdate(ManagerProfile);
			if (errors.Any())
			{
				return errors;
			}
			CompleteWithNotChangedOriginFiles(commandArg);
			_scriptStorage.SaveMashup(commandArg);
			_bus.Send(commandArg.CreatePluginMashupMessage());
			_log.InfoFormat("Update mashup commnad sent to TP (Mashup '{0}' for account '{1}')", commandArg.Name, _context.AccountName.Value);
			return new PluginProfileErrorCollection();
		}

		private void UpdateProfile(MashupManagerProfile managerProfile)
		{
			var addOrUpdateProfileCommand = ObjectFactory.GetInstance<AddOrUpdateProfileCommand>();
			var profileDto = new PluginProfileDto
			{
				Name = ProfileName,
				Settings = managerProfile
			};
			addOrUpdateProfileCommand.Execute(profileDto.Serialize());
		}

		private void CompleteWithNotChangedOriginFiles(UpdateMashupCommandArg commandArg)
		{
			var originMashup = _scriptStorage.GetMashup(commandArg.OldName);
			var filesToBeAdded = originMashup.Files
				.Where(x => !commandArg.Files.Any(y => y.FileName.Equals(x.FileName, StringComparison.InvariantCultureIgnoreCase)))
				.ToArray();

			if (filesToBeAdded.Any())
			{
				commandArg.Files.AddRange(filesToBeAdded);
			}
		}
	}
}
