// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using Tp.SourceControl.Settings;
using Tp.Tfs.WorkItemsIntegration.Exstentions;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Tp.Tfs.WorkItemsIntegration
{
    public class TfsWorkItemStoreClient
    {
        private readonly WorkItemStore _workItemStore;
        private readonly TfsCurrentProfileToConnectionSettingsAdapter _settings;

        public TfsWorkItemStoreClient(ISourceControlConnectionSettingsSource settings)
        {
            _settings = settings as TfsCurrentProfileToConnectionSettingsAdapter;
            _workItemStore = GetWorkItemStore(_settings);
        }

        public WorkItem[] GetWorkItemsFrom(string workItemNumber)
        {
            string query =
                $"SELECT * FROM workitems WHERE ([Team Project] = '{_settings.ProjectsMapping[0].Key}' AND [ID] >= {workItemNumber} AND [Work Item Type] IN ({_settings.EntityMapping.ToWorkItemsString()}))";
            var workItemsQuery = new Query(_workItemStore, query, null, false);
            var workItems = workItemsQuery.RunQuery();
            return workItems.Cast<WorkItem>().ToArray();
        }

        public WorkItem[] GetWorkItemsFrom(DateTime fromDate)
        {
            string query =
                $"SELECT * FROM workitems WHERE ([Team Project] = '{_settings.ProjectsMapping[0].Key}' AND [Changed Date] >= '{fromDate}' AND [Work Item Type] IN ({_settings.EntityMapping.ToWorkItemsString()}))";

            var workItemsQuery = new Query(_workItemStore, query, null, false);
            var workItems = workItemsQuery.RunQuery();
            return workItems.Cast<WorkItem>().ToArray();
        }

        public WorkItem GetWorkItem(int id)
        {
            return _workItemStore.GetWorkItem(id);
        }

        public WorkItem[] GetWorkItemsBetween(string projectName, string[] importedTypes, int minId, int maxId, DateTime lastSync)
        {
            string query =
                $"SELECT * FROM workitems WHERE ([Team Project] = '{projectName}' AND [ID] >= {minId} AND [ID] <= {(maxId == -1 ? Int32.MaxValue : maxId)} AND [Changed Date] > '{lastSync}'  AND [Work Item Type] IN ({importedTypes.ToString(x => string.Concat("'", x, "'"), ",")}))";

            var workItemsQuery = new Query(_workItemStore, query, null, false);
            var workItems = workItemsQuery.RunQuery();
            return workItems.Cast<WorkItem>().ToArray();
        }

        private static WorkItemStore GetWorkItemStore(ISourceControlConnectionSettingsSource settings)
        {
            var parameters = TfsConnectionHelper.GetTfsConnectionParameters(settings, out var _);

            var teamProjectCollection = new TfsTeamProjectCollection(parameters.TfsCollectionUri,
                new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt));
            teamProjectCollection.EnsureAuthenticated();

            var workItemStore = teamProjectCollection.GetService<WorkItemStore>();

            return workItemStore;
        }
    }
}
