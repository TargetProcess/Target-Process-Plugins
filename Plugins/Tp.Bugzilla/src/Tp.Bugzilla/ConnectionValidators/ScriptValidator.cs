// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.BugTracking.ConnectionValidators;
using Tp.BugTracking.Settings;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
    public class ScriptValidator : Validator, IDataHolder<string>
    {
        public ScriptValidator(IBugTrackingConnectionSettingsSource connectionSettings)
            : base(connectionSettings)
        {
        }

        protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
        {
            try
            {
                Data = new BugzillaUrl(ConnectionSettings).CheckConnection();
            }
            catch (Exception)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = BugzillaProfile.UrlField,
                    Message = "The Bugzilla script tp2.cgi was not found. Please copy tp2.cgi to the Bugzilla folder.",
                    AdditionalInfo = ValidationErrorType.TpCgiNotFound.ToString()
                });
            }
        }

        public string Data { get; private set; }
    }
}
