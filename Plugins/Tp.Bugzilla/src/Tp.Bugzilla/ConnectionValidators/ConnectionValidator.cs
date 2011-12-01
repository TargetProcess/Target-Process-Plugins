// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Net;
using System.Text;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
	public class ConnectionValidator : Validator
	{
		public ConnectionValidator(BugzillaProfile profile) : base(profile)
		{
		}

		protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
		{
			try
			{
				var url = new BugzillaUrl(_profile);
				var webClient = new WebClient {Encoding = Encoding.UTF8};
				webClient.DownloadString(url.Url);
			}
			catch (Exception bugzillaException)
			{
				errors.Add(new PluginProfileError {FieldName = BugzillaProfile.UrlField, Message = bugzillaException.Message, AdditionalInfo = ValidationErrorType.BugzillaNotFound.ToString()});
			}
		}
	}
}