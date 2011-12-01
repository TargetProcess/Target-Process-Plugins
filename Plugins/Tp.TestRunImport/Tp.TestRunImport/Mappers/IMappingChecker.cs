// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections.Generic;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Commands.Data;

namespace Tp.Integration.Plugin.TestRunImport.Mappers
{
	public interface IMappingChecker
	{
		CheckMappingResult CheckMapping(TestRunImportPluginProfile settings, IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans, PluginProfileErrorCollection errors);
	}
}