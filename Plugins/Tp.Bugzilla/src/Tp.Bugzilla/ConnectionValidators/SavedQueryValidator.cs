// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Core;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
	public class SavedQueryValidator : Validator, IDataHolder<int[]>
	{
		public SavedQueryValidator(BugzillaProfile profile) : base(profile)
		{
		}

		protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
		{
			try
			{
				Data = new BugzillaUrl(_profile).GetChangedBugsIds(CurrentDate.Value);
			}
			catch (Exception)
			{
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = BugzillaProfile.QueriesField,
				           		Message = "The defined Bugzilla Saved Searches are not valid",
								AdditionalInfo = ValidationErrorType.QueryNotFound.ToString()
				           	});
			}
		}

		public int[] Data { get; private set; }
	}
}