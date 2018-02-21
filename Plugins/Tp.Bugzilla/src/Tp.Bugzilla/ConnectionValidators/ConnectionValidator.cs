// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Net;
using System.Text;
using Tp.BugTracking.ConnectionValidators;
using Tp.BugTracking.Settings;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
    public class ConnectionValidator : Validator
    {
        public ConnectionValidator(IBugTrackingConnectionSettingsSource connectionSettings)
            : base(connectionSettings)
        {
        }

        protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
        {
            try
            {
                var url = new BugzillaUrl(ConnectionSettings);
                WebClient webClient = new TpWebClient(errors) { Encoding = Encoding.UTF8 };
                webClient.DownloadString(url.Url);
            }
            catch (WebException webException)
            {
                if (webException.Status != WebExceptionStatus.TrustFailure)
                {
                    errors.Add(new PluginProfileError
                    {
                        FieldName = BugzillaProfile.UrlField,
                        Message = webException.Message,
                        AdditionalInfo = ValidationErrorType.BugzillaNotFound.ToString()
                    });
                }
            }
            catch (Exception exception)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = BugzillaProfile.UrlField,
                    Message = exception.Message,
                    AdditionalInfo = ValidationErrorType.BugzillaNotFound.ToString()
                });
            }
        }
    }
}
