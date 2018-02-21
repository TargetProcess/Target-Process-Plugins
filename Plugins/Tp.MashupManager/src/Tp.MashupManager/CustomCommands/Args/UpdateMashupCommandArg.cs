using System;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager.CustomCommands.Args
{
    [DataContract, Serializable]
    public class UpdateMashupCommandArg : Mashup
    {
        [DataMember]
        public string OldName { get; set; }

        public bool IsNameChanged()
        {
            return !Name.Equals(OldName, StringComparison.InvariantCultureIgnoreCase);
        }

        public PluginProfileErrorCollection ValidateUpdate(MashupManagerProfile profile)
        {
            var errors = new PluginProfileErrorCollection();

            ValidateNameNotEmpty(errors);
            ValidateNameContainsOnlyValidChars(errors);

            return errors;
        }
    }
}
