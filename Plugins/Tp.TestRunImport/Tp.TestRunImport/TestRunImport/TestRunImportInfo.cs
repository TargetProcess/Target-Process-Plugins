// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImport
{
	[Serializable]
	public class TestRunImportInfo
	{
		public TestRunImportResultInfo[] TestRunImportResults { get; set; }
	}
}