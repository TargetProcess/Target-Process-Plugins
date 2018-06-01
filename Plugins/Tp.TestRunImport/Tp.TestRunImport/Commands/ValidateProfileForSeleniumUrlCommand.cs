// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Integration.Plugin.TestRunImport.Commands
{
    public class ValidateProfileForSeleniumUrlCommand : IPluginCommand
    {
        private readonly IProfileCollection _profileCollection;

        public ValidateProfileForSeleniumUrlCommand(IProfileCollection profileCollection)
        {
            _profileCollection = profileCollection;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            return new PluginCommandResponseMessage { ResponseData = OnExecute(args), PluginCommandStatus = PluginCommandStatus.Succeed };
        }

        private string OnExecute(string args)
        {
            var profile = args.DeserializeProfile(p => _profileCollection[p]);
            return ValidateProfileForSeleniumUrl(profile).Serialize();
        }

        private static PluginProfileErrorCollection ValidateProfileForSeleniumUrl(PluginProfileDto profileDto)
        {
            var errors = new PluginProfileErrorCollection();
            if (string.IsNullOrEmpty(profileDto.Name) || string.IsNullOrEmpty(profileDto.Name.Trim()))
            {
                errors.Add(new PluginProfileError { FieldName = "Name", Message = "Profile name should not be empty" });
            }
            ((TestRunImportPluginProfile) profileDto.Settings).ValidateSeleniumUrlData(errors);
            return errors;
        }

        public string Name
        {
            get { return "ValidateProfileForSeleniumUrl"; }
        }
    }
}
