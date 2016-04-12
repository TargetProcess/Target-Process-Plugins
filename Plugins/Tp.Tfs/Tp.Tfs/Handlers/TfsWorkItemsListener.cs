// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Tfs.WorkItemsIntegration;
using Tp.Tfs.WorkItemsIntegration.ChangedFieldsPolicy;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Messages;
using Tp.Tfs.WorkItemsIntegration.FieldsMapping;

namespace Tp.Tfs.Handlers
{
	public class TfsWorkItemsListener : IHandleMessages<TickMessage>
	{
		private readonly IWorkItemsStore _workItemsStore;
		private readonly ILocalBus _bus;
		private readonly IStorageRepository _storage;
		private readonly IActivityLogger _logger;

		public TfsWorkItemsListener(
				IWorkItemsStore workItemsStore,
				ILocalBus bus,
				IStorageRepository storage,
				IActivityLogger logger)
		{
			_workItemsStore = workItemsStore;
			_bus = bus;
			_storage = storage;
			_logger = logger;
		}

		public void Handle(TickMessage message)
		{
			if (!ConfigHelper.GetWorkItemsState())
				return;

			if (!IsSourceControlEnabled())
			{
				var tpBus = ObjectFactory.GetInstance<TpBus>();
				tpBus.DoNotContinueDispatchingCurrentMessageToHandlers();
			}

			if (!IsSynchronizationEnabled())
			{
				_logger.Info("Work items synchronization disabled");
				return;
			}

			_logger.Info("Syncronize work items");

			var messages = GetTargetWorkItemsMessages(message.LastSyncDate);
			messages.ForEach(msg => _bus.SendLocal(msg));
		}

		private bool IsSynchronizationEnabled()
		{
			var profile = _storage.GetProfile<TfsPluginProfile>();
			return profile.WorkItemsEnabled;
		}

		private bool IsSourceControlEnabled()
		{
			var profile = _storage.GetProfile<TfsPluginProfile>();
			return profile.SourceControlEnabled;
		}

		private bool IsFirstRun
		{
			get { return _storage.Get<WorkItemInfo>().Empty(); }
		}

		private IEnumerable<EntitySynchronizationMessage> GetTargetWorkItemsMessages(DateTime? lastSyncDate)
		{
			var workItems = new List<WorkItem>();
			var profile = _storage.GetProfile<TfsPluginProfile>();

			if (IsFirstRun)
			{
				var items = _workItemsStore.GetWorkItemsFrom(profile.StartWorkItem);
				workItems.AddRange(items);
			}
			else
			{
				var projectMappingHistory = _storage.Get<ProjectsMappingHistory>().FirstOrDefault();

				foreach (var projectMapping in projectMappingHistory)
				{
					foreach (var importType in projectMapping.ImportedTypes)
					{

						var items = _workItemsStore.GetWorkItemsBetween(
								projectMapping.Key,
								new[] { importType.Type },
								importType.StartID,
								projectMapping.IsCurrent ? -1 : projectMapping.WorkItemsRange.Max,
								importType.IsFirstSync ? ((DateTime)SqlDateTime.MinValue).AddDays(1) : lastSyncDate.Value);
						//lastSyncDate == null ? ((DateTime)SqlDateTime.MinValue).AddDays(1) : lastSyncDate.Value);

						workItems.AddRange(items);

						if (importType.IsFirstSync)
						{
							importType.IsFirstSync = false;
							_storage.Get<ProjectsMappingHistory>().ReplaceWith(projectMappingHistory);
						}
					}
				}
			}

			var checkedWorkItems = CreateSyncMessages(workItems.ToArray());
			return checkedWorkItems;
		}

		private IEnumerable<EntitySynchronizationMessage> CreateSyncMessages(IEnumerable<WorkItem> workItems)
		{
			var profile = _storage.GetProfile<TfsPluginProfile>();
			var savedWorkItems = _storage.Get<WorkItemInfo>().ToArray();

			var workItemsInfos = new List<WorkItemInfo>();
			foreach (WorkItem workItem in workItems)
			{
				var workItemInfo = new WorkItemInfo()
				{
					WorkItemId = workItem.ToWorkItemId(),
					WorkItemType = workItem.Type.Name,
					FieldsValues = WorkItemsUsedFields.Map(workItem),
					TfsProject = workItem.Project.Name
				};

				GetWorkItemSyncParameters(workItemInfo, savedWorkItems, profile.ProjectsMapping[0].Value);

				workItemsInfos.Add(workItemInfo);
			}

			FilterWorkItemsToUpdate(workItemsInfos);

			return workItemsInfos.Select(workItem => MapWorkItemToMessage(workItem, profile.EntityMapping)).ToList();
		}

		private void FilterWorkItemsToUpdate(List<WorkItemInfo> workItems)
		{
			var workItemsToUpdate = workItems.Where(x => x.Action == WorkItemAction.Update);
			var workItemsIds = workItemsToUpdate.Select(x => x.WorkItemId.Id);
			var relations =
					from info in _storage.Get<WorkItemInfo>()
					where workItemsIds.Contains(info.WorkItemId.Id)
					select new KeyValuePair<WorkItemInfo, WorkItemInfo>(info, workItemsToUpdate.First(x => info.WorkItemId.Equals(x.WorkItemId)));

			var profile = _storage.GetProfile<TfsPluginProfile>();

			foreach (var relation in relations)
			{
				CheckWorkItemChangedFields(relation.Key, relation.Value, profile);
				if (!relation.Value.ChangedFields.Any())
					workItems.Remove(relation.Value);
			}
		}

		private static bool IsFieldsEqual(WorkItemInfo workItem, WorkItemInfo workItemChanged, string field)
		{
			PropertyInfo workItemPropertyInfo = typeof(WorkItemInfo).GetProperty(field);

			if (workItemPropertyInfo == null)
				throw new Exception("Wrong fields mapping.");

			var workItemValue = workItemPropertyInfo.GetValue(workItem, null) as string;
			var changedValue = workItemPropertyInfo.GetValue(workItemChanged, null) as string;

			return workItemValue == changedValue;
		}

		private static void CheckWorkItemChangedFields(WorkItemInfo workItem, WorkItemInfo workItemChanged, TfsPluginProfile profile)
		{
			var changedFields = new List<Enum>();

			string tpType = profile.EntityMapping[workItem.WorkItemType];
			IChangedFieldsMappingPolicy mappingPolicy = ChangedFieldsPolicyFactory.CreatePolicy(tpType);

			foreach (var fieldValue in workItemChanged.FieldsValues)
			{
				var workItemField = workItem.FieldsValues.First(x => x.Name == fieldValue.Name);
				if (workItemField.Value != fieldValue.Value)
					changedFields.Add(mappingPolicy.WorkItemFieldToTpField(workItem.WorkItemType, fieldValue.Name));
			}

			workItemChanged.ChangedFields = changedFields.ToArray();
		}

		private static bool IsItemChanged(WorkItemInfo workItem, WorkItemInfo workItemChanged)
		{
			if (!IsFieldsEqual(workItem, workItemChanged, "Title"))
				return true;

			if (!IsFieldsEqual(workItem, workItemChanged, "Description"))
				return true;

			return false;
		}

		private EntitySynchronizationMessage MapWorkItemToMessage(
				WorkItemInfo workItemInfo,
				SimpleMappingContainer entitiesMap)
		{
			string tpEntity = entitiesMap[workItemInfo.WorkItemType];

			switch (tpEntity)
			{
				case Constants.Bug:
					return new BugSynchronizationMessage { WorkItem = workItemInfo };
				case Constants.UserStory:
					return new UserStorySynchronizationMessage { WorkItem = workItemInfo };
				case Constants.Feature:
					return new FeatureSynchronizationMessage { WorkItem = workItemInfo };
				case Constants.Request:
					return new RequestSynchronizationMessage { WorkItem = workItemInfo };
				default:
					throw new Exception(string.Format("There is no mapped entity type for {0}", workItemInfo.WorkItemType));
			}
		}

		private void GetWorkItemSyncParameters(
				WorkItemInfo workItem,
				IEnumerable<WorkItemInfo> savedWorkItems,
				MappingLookup currentProject)
		{
			var savedWorkItem = savedWorkItems.FirstOrDefault(x => x.WorkItemId.Equals(workItem.WorkItemId));

			if (savedWorkItem == null)
			{
				workItem.Action = WorkItemAction.Create;
				workItem.TpProjectId = currentProject.Id;
				workItem.TpProjectName = currentProject.Name;
			}
			else
			{
				workItem.Action = WorkItemAction.Update;
				workItem.TpProjectId = savedWorkItem.TpProjectId;
				workItem.TpProjectName = savedWorkItem.TpProjectName;
				workItem.TpEntityId = savedWorkItem.TpEntityId;
			}
		}
	}
}
