using System;
using System.Linq;
using log4net;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.MashupStorage;

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
            var saveErrors = MashupScriptStorageOperations.Save(_scriptStorage, dto);
            if (saveErrors.Any())
            {
                return saveErrors;
            }

            AddMashupToPluginProfile(dto.Name);
            _bus.Send(dto.CreatePluginMashupMessage());
            _log.InfoFormat("Add mashup command sent to TP (Mashup '{0}' for account '{1}')", dto.Name, _context.AccountName.Value);
            return errors;
        }

        public PluginProfileErrorCollection Update(UpdateMashupCommandArg commandArg)
        {
            if (commandArg.IsNameChanged())
            {
                UpdateUnchangedOriginFiles(commandArg);

                var errors = Add(commandArg, false);
                if (errors.Any())
                {
                    return errors;
                }

                var deleteErrors = Delete(commandArg.OldName);
                return deleteErrors;
            }

            return UpdateInternal(commandArg);
        }

        public PluginProfileErrorCollection Delete(string name)
        {
            var errors = MashupScriptStorageOperations.Delete(_scriptStorage, name);
            if (errors.Any())
            {
                return errors;
            }

            DeleteMashupFromPluginProfile(name);
            var dto = new Mashup { Name = name };
            _bus.Send(dto.CreatePluginMashupMessage());
            _log.InfoFormat("Clean mashup command sent to TP (Mashup '{0}' for account '{1}')", dto.Name, _context.AccountName.Value);
            return new PluginProfileErrorCollection();
        }

        private string GetUniqueMashupName(string mashupName)
        {
            var uniqueName = mashupName;
            var index = 1;

            while (ManagerProfile.ContainsMashupName(uniqueName))
            {
                uniqueName = "{0} {1}".Fmt(mashupName, index);
                index++;
            }

            return uniqueName;
        }

        private MashupManagerProfile ManagerProfile => _lazyMashupManagerProfile.Value;

        private void AddMashupToPluginProfile(string name)
        {
            ManagerProfile.AddMashup(name);
            UpdateProfile(ManagerProfile);
        }

        private void DeleteMashupFromPluginProfile(string name)
        {
            ManagerProfile.RemoveMashup(name);
            UpdateProfile(ManagerProfile);
        }

        private PluginProfileErrorCollection UpdateInternal(UpdateMashupCommandArg commandArg)
        {
            var errors = commandArg.ValidateUpdate(ManagerProfile);
            if (errors.Any())
            {
                return errors;
            }
            UpdateUnchangedOriginFiles(commandArg);
            var saveErrors = MashupScriptStorageOperations.Save(_scriptStorage, commandArg);
            if (saveErrors.Any())
            {
                return saveErrors;
            }
            _bus.Send(commandArg.CreatePluginMashupMessage());
            _log.InfoFormat("Update mashup command sent to TP (Mashup '{0}' for account '{1}')", commandArg.Name, _context.AccountName.Value);
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
            addOrUpdateProfileCommand.Execute(profileDto.Serialize(), null);
        }

        private void UpdateUnchangedOriginFiles(UpdateMashupCommandArg commandArg)
        {
            var originalMashup = _scriptStorage.GetMashup(commandArg.OldName);
            var filesToBeAdded = originalMashup.Files
                .Where(x => !commandArg.Files.Any(y => y.FileName.Equals(x.FileName, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();

            if (filesToBeAdded.Any())
            {
                commandArg.Files.AddRange(filesToBeAdded);
            }
        }
    }
}
