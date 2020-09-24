// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Tp.SourceControl.Settings;

namespace Tp.Tfs.WorkItemsIntegration
{
    public class TfsWorkItemsStore : IWorkItemsStore
    {
        private readonly TfsWorkItemStoreClient _workItemStore;

        public TfsWorkItemsStore(ISourceControlConnectionSettingsSource settings)
        {
            _workItemStore = ((TfsCurrentProfileToConnectionSettingsAdapter)settings).WorkItemsEnabled ? new TfsWorkItemStoreClient(settings) : null;
        }

        public WorkItem[] GetWorkItemsFrom(string workItemNumber)
        {
            return _workItemStore != null ? _workItemStore.GetWorkItemsFrom(workItemNumber) : new WorkItem[]{};
        }

        public WorkItem[] GetWorkItemsFrom(DateTime from)
        {
            return _workItemStore != null ? _workItemStore.GetWorkItemsFrom(from) : new WorkItem[]{};
        }

        public WorkItem[] GetWorkItemsBetween(string projectName, string[] importedTypes, int minId, int maxId, DateTime lastSync)
        {
            return _workItemStore != null ? _workItemStore.GetWorkItemsBetween(projectName, importedTypes, minId, maxId, lastSync) : new WorkItem[]{};
        }

        public WorkItem GetWorkItem(int id)
        {
            return _workItemStore?.GetWorkItem(id);
        }
    }
}
