// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Integration.Plugin.TestRunImport.Mappers
{
	public class MappingProfileProfileValidator : IMappingProfileValidator
	{
		public void ValidateProfileForMapping(TestRunImportPluginProfile settings, PluginProfileErrorCollection errors)
		{
			settings.ValidateMapperData(errors);
		}
	}
}