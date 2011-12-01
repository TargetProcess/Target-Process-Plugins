// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;

namespace Tp.TestRunImport.Tests.NUnit
{
	[ActionSteps]
	public class NUnitTestRunImportActionSteps : ImportResultsTestRunImportActionSteps
	{
		protected override FrameworkTypes FrameworkType
		{
			get { return FrameworkTypes.NUnit; }
		}
	}
}