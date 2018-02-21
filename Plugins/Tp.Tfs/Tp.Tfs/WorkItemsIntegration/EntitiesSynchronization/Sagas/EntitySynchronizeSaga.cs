// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using NServiceBus;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Entities;
using Tp.Tfs.WorkItemsIntegration.FieldsMapping;

namespace Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Sagas
{
    public abstract class EntitySynchronizeSaga<TWorkItemEntity, TDto> : TpSaga<TWorkItemEntity>
        where TWorkItemEntity : WorkItemEntity
        where TDto : DataTransferObject, new()
    {
        protected IActivityLogger Logger;

        protected void ConfigureHowToFindSagaInternal<TDto, TDtoField, TEntityCreatedMessage, TEntityUpdatedMessage>()
            where TDto : DataTransferObject, new()
            where TEntityCreatedMessage : EntityCreatedMessage<TDto>, IMessage
            where TEntityUpdatedMessage : EntityUpdatedMessage<TDto, TDtoField>, IMessage
        {
            ConfigureMapping<TEntityCreatedMessage>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<TEntityUpdatedMessage>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
        }

        protected void HandleInternal(WorkItemInfo itemInfo)
        {
            Data.WorkItem = itemInfo;
            TDto entityDto = CreateEntityDTO(itemInfo);

            if (itemInfo.Action == WorkItemAction.Create)
            {
                Logger.InfoFormat("Creating {0}: {1}", itemInfo.WorkItemType, itemInfo.ToString());

                SendCreateCommand(entityDto);
            }
            else if (itemInfo.Action == WorkItemAction.Update)
            {
                Logger.InfoFormat("Updating {0}: {1}", itemInfo.WorkItemType, itemInfo.ToString());

                if (itemInfo.TpEntityId == null)
                {
                    var workItemField = itemInfo.FieldsValues.FirstOrDefault(x => x.Name == "Title");

                    Logger.WarnFormat(
                        "Relation for Work Item '{0} {1}' not found in profile storage.",
                        itemInfo.WorkItemId.Id,
                        workItemField == null ? string.Empty : workItemField.Value);

                    DoNotContinueDispatchingCurrentMessageToHandlers();
                    MarkAsComplete();

                    return;
                }

                entityDto.ID = int.Parse(itemInfo.TpEntityId.Id);
                SendUpdateCommand(entityDto, itemInfo);
            }
        }

        protected void HandleCreatedInternal<TEntityCreatedMessage>(TEntityCreatedMessage message)
            where TEntityCreatedMessage : EntityCreatedMessage<TDto>
        {
            var tpEntityId = message.Dto.ID;

            var storage = ObjectFactory.GetInstance<IStorageRepository>();
            Data.WorkItem.TpEntityId = new TpEntityId
            {
                Id = tpEntityId.GetValueOrDefault().ToString(CultureInfo.InvariantCulture),
                Type = typeof(TDto).FullName
            };

            storage.Get<WorkItemInfo>().Add(Data.WorkItem);

            var projectsMappingHistory = storage.Get<ProjectsMappingHistory>().FirstOrDefault();

            if (projectsMappingHistory != null)
            {
                int newId = int.Parse(Data.WorkItem.WorkItemId.Id);
                if (projectsMappingHistory.Current.WorkItemsRange.Max < newId)
                    projectsMappingHistory.Current.WorkItemsRange.Max = newId;

                storage.Get<ProjectsMappingHistory>().ReplaceWith(projectsMappingHistory);
            }

            Logger.InfoFormat(
                "{0} created: {1}; TargetProcess entity ID: {2}",
                Data.WorkItem.WorkItemType,
                Data.WorkItem.ToString(),
                message.Dto.ID);

            MarkAsComplete();
        }

        protected void HandleUpdatedInternal<TEntityUpdatedMessage, TEnum>(TEntityUpdatedMessage message)
            where TEntityUpdatedMessage : EntityUpdatedMessage<TDto, TEnum>
        {
            Logger.InfoFormat(
                "{0} updated: {1}; TargetProcess entity ID: {2}",
                Data.WorkItem.WorkItemType,
                Data.WorkItem.ToString(),
                message.Dto.ID);

            var storage = ObjectFactory.GetInstance<IStorageRepository>();
            storage.Get<WorkItemInfo>(Data.WorkItem.WorkItemId.Id).ReplaceWith(Data.WorkItem);

            DoNotContinueDispatchingCurrentMessageToHandlers();
            MarkAsComplete();
        }

        protected void HandleErrorInternal(TargetProcessExceptionThrownMessage message)
        {
            if (!string.IsNullOrEmpty(message.ExceptionString) && message.ExceptionString.StartsWith("EntityNotFoundException"))
                Logger.Warn(string.Format("Entity {0} {1} not found in Target Porcess", Data.WorkItem.TpEntityId.Id,
                    Data.WorkItem.TpEntityId.Type));
            else
                Logger.Error("Error occured", new Exception(message.ExceptionString));

            MarkAsComplete();
        }

        protected abstract void SendUpdateCommand(TDto entityDTO, WorkItemInfo itemInfo);
        protected abstract void SendCreateCommand(TDto entityDTO);

        protected virtual TDto CreateEntityDTO(WorkItemInfo workItem)
        {
            var tpEntityDto = new TDto();

            var mapping = WorkItemsFieldsMapper.Instance.GetMappingForWorkItemType(workItem.WorkItemType);

            InitFields(tpEntityDto, workItem, mapping);

            return tpEntityDto;
        }

        private void InitFields(
            TDto tpEntityDto,
            WorkItemInfo workItem,
            Dictionary<string, WorkItemsFieldsMapper.NamesPair[]> fieldsMapping)
        {
            foreach (var mapping in fieldsMapping)
                InitField(tpEntityDto, workItem, mapping);
        }

        private void InitField(
            TDto tpEntityDto,
            WorkItemInfo workItem,
            KeyValuePair<string, WorkItemsFieldsMapper.NamesPair[]> fieldMapping)
        {
            var dtoProperty = tpEntityDto.GetType().GetProperty(fieldMapping.Key);

            if (dtoProperty == null)
                return;

            var values = new List<string>();

            foreach (var field in fieldMapping.Value)
            {
                var workItemField = workItem.FieldsValues.FirstOrDefault(x => x.Name == field.Name);
                if (workItemField != null && !string.IsNullOrWhiteSpace(workItemField.Value))
                {
                    if (fieldMapping.Value.Count() > 1)
                    {
                        values.Add(string.Concat("<b>", field.VisualName.ToUpper(), "</b>"));
                        values.Add("<br/>");

                        string validated = ValidateString(workItemField);
                        values.Add(validated);

                        values.Add("<br/><br/>");

                        continue;
                    }

                    string singleValidated = ValidateString(workItemField);
                    values.Add(singleValidated);
                }
            }

            if (values.Any())
            {
                string totalValue = string.Join(string.Empty, values);
                dtoProperty.SetValue(tpEntityDto, totalValue, null);
            }
        }

        private string ValidateString(WorkItemField field)
        {
            if (field.Type == Constants.FieldTypePlainText)
                return ValidateString(field.Value);

            return field.Value;
        }

        private string ValidateString(string value)
        {
            string encoded = HttpUtility.HtmlEncode(value);
            var splited = encoded.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var validated = splited.Select(x => string.Concat("<P>", x, "</P>")).ToArray();
            return string.Join(string.Empty, validated);
        }

        protected bool IsCurrentProjectMapping(TfsPluginProfile profile, WorkItemInfo info)
        {
            return info.TpProjectId == profile.ProjectsMapping[0].Value.Id &&
                info.TfsProject == profile.ProjectsMapping[0].Key;
        }
    }
}
