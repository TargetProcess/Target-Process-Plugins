// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla
{
	public static class BugzillaInfo
	{
		public static string[] SupportedVersions
		{
			get
			{
				return new[]
				{
					"3.4",
					"3.6",
					"4.0",
					"4.2"
				};
			}
		}

		public static string SupportedCgiScriptVersion
		{
			get { return "2"; }
		}
	}
}