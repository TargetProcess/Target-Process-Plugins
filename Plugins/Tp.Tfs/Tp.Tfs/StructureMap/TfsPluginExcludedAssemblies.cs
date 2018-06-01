// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
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
                    "Microsoft.IdentityModel.Clients.ActiveDirectory.dll",
                    "Microsoft.IdentityModel.Clients.ActiveDirectory.Platform.dll",
                    "Microsoft.Practices.ServiceLocation.dll",
                    "Microsoft.ServiceBus.dll",
                    "Microsoft.TeamFoundation.Build.Client.dll",
                    "Microsoft.TeamFoundation.Build.Common.dll",
                    "Microsoft.TeamFoundation.Build2.WebApi.dll",
                    "Microsoft.TeamFoundation.Chat.WebApi.dll",
                    "Microsoft.TeamFoundation.Client.dll",
                    "Microsoft.TeamFoundation.Common.dll",
                    "Microsoft.TeamFoundation.Core.WebApi.dll",
                    "Microsoft.TeamFoundation.Dashboards.WebApi.dll",
                    "Microsoft.TeamFoundation.DeleteTeamProject.dll",
                    "Microsoft.TeamFoundation.Diff.dll",
                    "Microsoft.TeamFoundation.Discussion.Client.dll",
                    "Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll",
                    "Microsoft.TeamFoundation.Git.Client.dll",
                    "Microsoft.TeamFoundation.Lab.Client.dll",
                    "Microsoft.TeamFoundation.Lab.Common.dll",
                    "Microsoft.TeamFoundation.Lab.TestIntegration.Client.dll",
                    "Microsoft.TeamFoundation.Lab.WorkflowIntegration.Client.dll",
                    "Microsoft.TeamFoundation.Policy.WebApi.dll",
                    "Microsoft.TeamFoundation.ProjectManagement.dll",
                    "Microsoft.TeamFoundation.SharePointReporting.Integration.dll",
                    "Microsoft.TeamFoundation.SourceControl.WebApi.dll",
                    "Microsoft.TeamFoundation.Test.WebApi.dll",
                    "Microsoft.TeamFoundation.TestImpact.Client.dll",
                    "Microsoft.TeamFoundation.TestManagement.Client.dll",
                    "Microsoft.TeamFoundation.TestManagement.Common.dll",
                    "Microsoft.TeamFoundation.TestManagement.WebApi.dll",
                    "Microsoft.TeamFoundation.VersionControl.Client.dll",
                    "Microsoft.TeamFoundation.VersionControl.Common.dll",
                    "Microsoft.TeamFoundation.VersionControl.Common.Integration.dll",
                    "Microsoft.TeamFoundation.Work.WebApi.dll",
                    "Microsoft.TeamFoundation.WorkItemTracking.Client.dll",
                    "Microsoft.TeamFoundation.WorkItemTracking.Client.DataStoreLoader.dll",
                    "Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll",
                    "Microsoft.TeamFoundation.WorkItemTracking.Common.dll",
                    "Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll",
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll",
                    "Microsoft.VisualStudio.Services.Client.Interactive.dll",
                    "Microsoft.VisualStudio.Services.Common.dll",
                    "Microsoft.VisualStudio.Services.WebApi.dll",
                    "Microsoft.Web.Services3.dll",
                    "Microsoft.WindowsAzure.Configuration.dll",
                    "Microsoft.WITDataStore32.dll",
                    "Microsoft.WITDataStore64.dll",
                    "System.IdentityModel.Tokens.Jwt.dll",
                    "System.Net.Http.Formatting.dll",
                });
        }
    }
}
