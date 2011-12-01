// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Mappers;

namespace Tp.Integration.Plugin.TestRunImport.Commands.Data
{
	public class CheckMappingResult
	{
		public int Mapped { get; set; }
		public int NotMappedTestCases { get; set; }
		public int NotMappedUnitTests { get; set; }
		public int OverMappedUnitTests { get; set; }
		public IList<NamesMapper> NamesMappers { get; set; }
		public PluginProfileErrorCollection Errors { get; set; }
	}
}