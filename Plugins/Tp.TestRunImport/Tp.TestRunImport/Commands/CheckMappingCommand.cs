// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Commands.Data;
using Tp.Integration.Plugin.TestRunImport.Mappers;

namespace Tp.Integration.Plugin.TestRunImport.Commands
{
	public class CheckMappingCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			return new PluginCommandResponseMessage
			       	{ResponseData = OnExecute(args), PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		private static string OnExecute(string args)
		{
			var data = args.Deserialize(typeof (CheckMappingData));

			return
				CheckMapping((TestRunImportPluginProfile) (((CheckMappingData) data).Profile).Settings,
				             ((CheckMappingData) data).TestCases).Serialize();
		}

		private static CheckMappingResult CheckMapping(TestRunImportPluginProfile settings,
		                                               IEnumerable<TestCaseLightDto> testCases)
		{
			var testCaseTestPlanDtos =
				testCases.Select(
					testCaseLightDto =>
					new TestCaseTestPlanDTO {TestCaseID = testCaseLightDto.Id, TestCaseName = testCaseLightDto.Name}).ToList();
			var errors = new PluginProfileErrorCollection();
			settings.ValidateMapperData(errors);
			return ObjectFactory.GetInstance<IMappingChecker>().CheckMapping(settings, testCaseTestPlanDtos, errors);
		}

		public string Name
		{
			get { return "CheckMapping"; }
		}
	}
}