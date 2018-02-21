using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager
{
    [DataContract, Serializable]
    public class Mashup
    {
        public const string AccountCfgFileName = "account.cfg";
        public const string NameField = "Name";

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Placeholders { get; set; }

        [DataMember]
        public MashupMetaInfo MashupMetaInfo { get; set; }

        /// <summary>
        /// Mashup files except of config files
        /// </summary>
        [DataMember]
        public List<MashupFile> Files { get; private set; }

        public Mashup(List<MashupFile> files = null)
        {
            MashupMetaInfo = new MashupMetaInfo
            {
                IsEnabled = true
            };
            Files = files ?? new List<MashupFile>();
        }

        #region Validation

        public PluginProfileErrorCollection ValidateAdd(MashupManagerProfile profile)
        {
            var errors = new PluginProfileErrorCollection();

            ValidateNameNotEmpty(errors);
            ValidateNameContainsOnlyValidChars(errors);
            ValidateNameUniqueness(errors, profile);

            return errors;
        }

        protected void ValidateNameNotEmpty(PluginProfileErrorCollection errors)
        {
            if (string.IsNullOrWhiteSpace(Name))
                errors.Add(new PluginProfileError
                {
                    FieldName = NameField,
                    Message = "Mashup name cannot be empty or consist of whitespace characters only"
                });
        }

        protected void ValidateNameContainsOnlyValidChars(PluginProfileErrorCollection errors)
        {
            if (!ProfileDtoValidator.IsValid(Name))
                errors.Add(new PluginProfileError
                {
                    FieldName = NameField,
                    Message = "You can only use letters, numbers, space and underscore symbol in Mashup name"
                });
        }

        protected void ValidateNameUniqueness(PluginProfileErrorCollection errors, MashupManagerProfile profile)
        {
            if (errors.Any())
            {
                return;
            }

            if (profile != null && profile.ContainsMashupName(Name))
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = NameField,
                    Message = "Mashup with the same name already exists"
                });
            }
        }

        #endregion

        #region Creation

        public PluginMashupMessage CreatePluginMashupMessage()
        {
            return CreatePluginMashupMessage(ObjectFactory.GetInstance<IPluginContext>().AccountName);
        }

        public PluginMashupMessage CreatePluginMashupMessage(AccountName accountName)
        {
            return new PluginMashupMessage
            {
                PluginMashupScripts = GetMashupScripts(accountName),
                PluginName = string.Empty,
                Placeholders = GetPlaceholders(),
                MashupName = Name,
                AccountName = accountName,
                MashupMetaInfo = MashupMetaInfo
            };
        }

        #endregion

        #region Private methods

        private string[] GetPlaceholders()
        {
            return Placeholders == null
                ? new string[] { }
                : Placeholders.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }

        private PluginMashupScript[] GetMashupScripts(AccountName accountName)
        {
            var scripts = Files.Select(f => new PluginMashupScript { FileName = f.FileName, ScriptContent = f.Content }).ToList();
            if (!scripts.Empty())
            {
                AppendAccountMashupFile(accountName, scripts);
            }
            return scripts.ToArray();
        }

        private void AppendAccountMashupFile(AccountName accountName, List<PluginMashupScript> scripts)
        {
            if (accountName != AccountName.Empty)
            {
                scripts.Add(new PluginMashupScript
                {
                    FileName = AccountCfgFileName,
                    ScriptContent = MashupConfig.AccountsConfigLine(accountName)
                });
            }
        }

        #endregion
    }
}
