// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.BugTracking.ConnectionValidators;
using Tp.BugTracking.Settings;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
    public class DeserializeValidator : Validator, IDataHolder<bugzilla_properties>
    {
        private readonly IDataHolder<string> _dataHolder;

        public DeserializeValidator(IBugTrackingConnectionSettingsSource connectionSettings, IDataHolder<string> dataHolder)
            : base(connectionSettings)
        {
            _dataHolder = dataHolder;
        }

        protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
        {
            try
            {
                Data = new BugzillaParser<bugzilla_properties>().Parse(_dataHolder.Data);
            }
            catch (InvalidOperationException)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = BugzillaProfile.LoginField,
                    Message = "The credentials are invalid.",
                    AdditionalInfo = ValidationErrorType.InvalidCredentials.ToString()
                });
                errors.Add(new PluginProfileError
                {
                    FieldName = BugzillaProfile.PasswordField,
                    Message = "The credentials are invalid.",
                    AdditionalInfo = ValidationErrorType.InvalidCredentials.ToString()
                });
            }
        }

        public bugzilla_properties Data { get; private set; }
    }
}
