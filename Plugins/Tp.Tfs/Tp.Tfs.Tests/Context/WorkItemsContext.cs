// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.VisualStudio.Services.Common;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Testing.Common;
using Tp.Tfs.Tests.WorkItems;
using Tp.Tfs.WorkItemsIntegration;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Tp.Tfs.Tests.Context
{
    public class WorkItemsContext
    {
        private TfsTeamProjectCollection _tfsCollection;
        private WorkItemStore _workItemStore;
        private readonly List<int> _ids = new List<int>();
        private readonly List<BugDTO> _bugs = new List<BugDTO>();
        private readonly List<UserStoryDTO> _userStories = new List<UserStoryDTO>();
        private readonly List<FeatureDTO> _features = new List<FeatureDTO>();
        private readonly List<RequestDTO> _requests = new List<RequestDTO>();

        public WorkItemsContext()
        {
            EntityMapping = new SimpleMappingContainer()
            {
                new SimpleMappingElement { First = "Bug", Second = "Bug" },
                new SimpleMappingElement { First = "User Story", Second = "User Story" },
                new SimpleMappingElement { First = "Task", Second = "Feature" },
                new SimpleMappingElement { First = "Issue", Second = "Request" }
            };
            ProjectsMapping = new MappingContainer()
            {
                new MappingElement()
                {
                    Key = ConfigHelper.Instance.TestCollectionProject,
                    Value = new MappingLookup() { Id = 1, Name = "Test" }
                }
            };
            WorkItems = new List<WorkItem>();
            Uri = ConfigHelper.Instance.TestCollection;
            Credential = new NetworkCredential(ConfigHelper.Instance.Login, ConfigHelper.Instance.Password, ConfigHelper.Instance.Domen);
            _tfsCollection = new TfsTeamProjectCollection(new Uri(Uri),
                new VssCredentials(new WindowsCredential(Credential), CredentialPromptType.DoNotPrompt));
            _workItemStore = _tfsCollection.GetService<WorkItemStore>();

            ObjectFactory.Configure(x => x.AddRegistry<WorkItemsUnitTestsRegistry>());
            AddReplyForCreateCommand<BugDTO, BugCreatedMessage>(_bugs);
            AddReplyForCreateCommand<UserStoryDTO, UserStoryCreatedMessage>(_userStories);
            AddReplyForCreateCommand<FeatureDTO, FeatureCreatedMessage>(_features);
            AddReplyForCreateCommand<RequestDTO, RequestCreatedMessage>(_requests);

            AddReplyForUpdateCommand<UserStoryDTO, UserStoryField, UserStoryUpdatedMessage>(TpUserStories, UserStoryField.EntityStateID);
        }

        public void AddProfile(string profileName)
        {
            TransportMock.AddProfile(profileName, GetTfsPluginProfile());
        }

        public SimpleMappingContainer EntityMapping { get; set; }
        public MappingContainer ProjectsMapping { get; set; }
        public List<WorkItem> WorkItems { get; set; }
        public string Uri { get; set; }
        public NetworkCredential Credential { get; set; }

        public List<BugDTO> TpBugs => _bugs;

        public List<UserStoryDTO> TpUserStories => _userStories;

        public List<FeatureDTO> TpFeatures => _features;

        public List<RequestDTO> TpRequests => _requests;

        public string TeamProject => ProjectsMapping[0].Key;

        public MappingLookup TpProject => ProjectsMapping[0].Value;

        public void AddWorkItem(string title, string description, string type)
        {
            var workItemType = _workItemStore.Projects[TeamProject].WorkItemTypes[type];
            var workItem = new WorkItem(workItemType)
            {
                Title = title,
                Description = description
            };

            workItem.Save();
        }

        public WorkItem GetWorkItem(string type, string title, string project)
        {
            var workItems = _workItemStore.Query(
                $"SELECT * FROM workitems WHERE [Work Item Type]='{type}' and [Team Project]='{project}' and [Title]='{title}'");
            var workItem = workItems.Cast<WorkItem>().FirstOrDefault(x => x.Title == title && x.Project.Name == project);
            return workItem;
        }

        public void ChangeWorkItem(string name, Dictionary<string, string> fieldsValues)
        {
            var workItems = _workItemStore.Query($"SELECT * FROM workitems WHERE [Title]='{name}'");

            var workItem = workItems.Cast<WorkItem>().FirstOrDefault();

            foreach (var fieldValue in fieldsValues)
            {
                PropertyInfo propertyInfo = typeof(WorkItem).GetProperty(fieldValue.Key);
                propertyInfo.SetValue(workItem, fieldValue.Value, null);
            }

            workItem.Save();
        }

        private static TransportMock TransportMock => ObjectFactory.GetInstance<TransportMock>();

        private TfsPluginProfile GetTfsPluginProfile()
        {
            return new TfsPluginProfile
            {
                Login = ConfigHelper.Instance.Login,
                Password = ConfigHelper.Instance.Password,
                Uri = ConfigHelper.Instance.TestCollection,
                SourceControlEnabled = false,
                WorkItemsEnabled = true,
                EntityMapping = EntityMapping,
                ProjectsMapping = ProjectsMapping
            };
        }

        public IProfileReadonly Profile => ObjectFactory.GetInstance<IProfileReadonly>();

        public void ClearWorkItems()
        {
            var workItems = _workItemStore.Query("SELECT * FROM workitems").Cast<WorkItem>().Select(x => x.Id).ToList();
            var errors = _workItemStore.DestroyWorkItems(workItems).Cast<WorkItemOperationError>().ToList();
        }

        public void DisposeTfs()
        {
            _tfsCollection.Dispose();
        }

        private void AddReplyForCreateCommand<TDto, TReplyMessage>(ICollection<TDto> dtos)
            where TReplyMessage : EntityCreatedMessage<TDto>, ISagaMessage, new()
            where TDto : DataTransferObject, new()
        {
            TransportMock.OnCreateEntityCommand<TDto>().Reply(
                x =>
                {
                    x.ID = GetNextId();
                    dtos.Add(x);
                    return new TReplyMessage { Dto = x };
                });
        }

        private void AddReplyForUpdateCommand<TDto, TEntityField, TReplyMessage>(
            ICollection<TDto> dtos,
            params TEntityField[] changedFields)
            where TReplyMessage : EntityUpdatedMessage<TDto, TEntityField>, ISagaMessage, new()
            where TEntityField : IConvertible
            where TDto : DataTransferObject, new()
        {
            TransportMock.OnUpdateEntityCommand<TDto>().Reply(
                x =>
                {
                    dtos.Remove(dtos.Single(storedEntity => storedEntity.ID == x.ID));
                    dtos.Add(x);
                    return new TReplyMessage { Dto = x, ChangedFields = changedFields };
                });
        }

        private int GetNextId()
        {
            var result = !_ids.Any() ? 1 : _ids.Max() + 1;
            _ids.Add(result);
            return result;
        }
    }
}
