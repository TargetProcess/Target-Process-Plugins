// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
	public abstract class Validator : IValidator
	{
		protected BugzillaProfile _profile;

		protected Validator(BugzillaProfile profile)
		{
			_profile = profile;
		}

		public void Execute(PluginProfileErrorCollection errors)
		{
			if (errors.Any())
				return;

			ExecuteConcreate(errors);
		}

		protected abstract void ExecuteConcreate(PluginProfileErrorCollection errors);
	}
}