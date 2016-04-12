// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Tp.SourceControl.Settings;
using Tp.Tfs.WorkItemsIntegration.Exstentions;

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
			string query = string.Format(
					"SELECT * FROM workitems WHERE ([Team Project] = '{0}' AND [ID] >= {1} AND [Work Item Type] IN ({2}))",
					_settings.ProjectsMapping[0].Key,
					workItemNumber,
					_settings.EntityMapping.ToWorkItemsString());
			var workItemsQuery = new Query(_workItemStore, query, null, false);
			var workItems = workItemsQuery.RunQuery();
			return workItems.Cast<WorkItem>().ToArray();
		}

		public WorkItem[] GetWorkItemsFrom(DateTime fromDate)
		{
			string query = string.Format(
					"SELECT * FROM workitems WHERE ([Team Project] = '{0}' AND [Changed Date] >= '{1}' AND [Work Item Type] IN ({2}))",
					_settings.ProjectsMapping[0].Key,
					fromDate,
					_settings.EntityMapping.ToWorkItemsString());

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
			string query = string.Format(
					"SELECT * FROM workitems WHERE ([Team Project] = '{0}' AND [ID] >= {1} AND [ID] <= {2} AND [Changed Date] > '{3}'  AND [Work Item Type] IN ({4}))",
					projectName,
					minId,
					maxId == -1 ? Int32.MaxValue : maxId,
					lastSync,
					importedTypes.ToString(x => string.Concat("'", x, "'"), ","));

			var workItemsQuery = new Query(_workItemStore, query, null, false);
			var workItems = workItemsQuery.RunQuery();
			return workItems.Cast<WorkItem>().ToArray();
		}

		private static WorkItemStore GetWorkItemStore(ISourceControlConnectionSettingsSource settings)
		{
			TfsConnectionParameters parameters = TfsConnectionHelper.GetTfsConnectionParameters(settings);

			var teamProjectCollection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);
			teamProjectCollection.EnsureAuthenticated();

			var workItemStore = teamProjectCollection.GetService<WorkItemStore>();

			return workItemStore;
		}
	}
}
