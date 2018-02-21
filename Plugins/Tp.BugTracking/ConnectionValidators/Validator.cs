// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.BugTracking.Settings;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.BugTracking.ConnectionValidators
{
    public abstract class Validator : IValidator
    {
        protected Validator(IBugTrackingConnectionSettingsSource connectionSettings)
        {
            ConnectionSettings = connectionSettings;
        }

        public IBugTrackingConnectionSettingsSource ConnectionSettings { get; }

        public void Execute(PluginProfileErrorCollection errors)
        {
            if (errors.Any())
                return;

            ExecuteConcreate(errors);
        }

        protected abstract void ExecuteConcreate(PluginProfileErrorCollection errors);
    }
}
