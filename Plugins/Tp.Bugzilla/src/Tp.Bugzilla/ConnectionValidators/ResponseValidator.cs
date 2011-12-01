// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
	public class ResponseValidator : Validator, IDataHolder<string>
	{
		private readonly IDataHolder<string> _dataHolder;

		public ResponseValidator(BugzillaProfile profile, IDataHolder<string> dataHolder) : base(profile)
		{
			_dataHolder = dataHolder;
		}

		protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
		{
			if (!string.IsNullOrEmpty(_dataHolder.Data) && _dataHolder.Data.Trim().ToLower() == "ok")
			{
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = BugzillaProfile.UrlField,
				           		Message =
				           			"The 'tp2.cgi' script version is not supported by this plugin. Please update your 'tp2.cgi' script and try again.",
									AdditionalInfo = ValidationErrorType.InvalidTpCgiVersion.ToString()
				           	});
			}

			Data = _dataHolder.Data;
		}

		public string Data { get; private set; }
	}
}