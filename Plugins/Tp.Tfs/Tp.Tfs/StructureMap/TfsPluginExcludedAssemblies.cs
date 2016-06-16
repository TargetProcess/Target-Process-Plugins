// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Plugin.Common;

namespace Tp.Tfs.StructureMap
{
	public class TfsPluginExcludedAssemblies : List<string>, IExcludedAssemblyNamesSource
	{
		public TfsPluginExcludedAssemblies()
		{
			AddRange(
					new[]
					{
						"Microsoft.TeamFoundation.WorkItemTracking.Client.Cache.dll", 
						"Microsoft.TeamFoundation.WorkItemTracking.Client.DataStore.dll",
						"Microsoft.TeamFoundation.WorkItemTracking.Client.RuleEngine.dll",
						"Microsoft.TeamFoundation.VersionControl.Common.Integration.dll",
						"Microsoft.TeamFoundation.WorkItemTracking.Client.dll",
						"Microsoft.TeamFoundation.WorkItemTracking.Common.dll",
						"Microsoft.TeamFoundation.WorkItemTracking.Client.Provision.dll",
						"Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll",
						"Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll",
						"Microsoft.TeamFoundation.Diff.dll",
						"Microsoft.WITDataStore32.dll",
						"Microsoft.WITDataStore64.dll",
						"Microsoft.VisualStudio.Services.Client.dll",
						"Microsoft.VisualStudio.Services.Common.dll",
						"Microsoft.VisualStudio.Services.WebApi.dll",
						"System.Net.Http.Formatting.dll"
					});
		}
	}
}
