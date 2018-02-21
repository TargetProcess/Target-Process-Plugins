// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Tfs.WorkItemsIntegration;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Tfs.Handlers
{
    public class TfsProfileUpdatedHandler : IHandleMessages<ProfileUpdatedMessage>
    {
        private readonly IProfile _profile;

        public TfsProfileUpdatedHandler(IProfile profile)
        {
            _profile = profile;
        }

        public void Handle(ProfileUpdatedMessage message)
        {
            var tfsProfile = _profile.Settings as TfsPluginProfile;

            var projectsMappingHistory = _profile.Get<ProjectsMappingHistory>().FirstOrDefault();

            if (projectsMappingHistory == null)
                return;

            var currentElement = projectsMappingHistory.Current;
            var importedTypes = new List<ImportedType>(currentElement.ImportedTypes);

            foreach (var mapping in tfsProfile.EntityMapping)
            {
                if (currentElement.ImportedTypes.Exists(x => x.Type == mapping.First))
                    continue;

                importedTypes.Add(new ImportedType
                {
                    StartID = int.Parse(tfsProfile.StartWorkItem),
                    Type = mapping.First,
                    IsFirstSync = true
                });
            }

            if (currentElement.IsEquals(tfsProfile.ProjectsMapping[0]))
            {
                if (importedTypes.Count != currentElement.ImportedTypes.Count)
                {
                    currentElement.ImportedTypes = importedTypes;
                    _profile.Get<ProjectsMappingHistory>().ReplaceWith(projectsMappingHistory);
                }

                return;
            }

            currentElement.IsCurrent = false;

            importedTypes = importedTypes.Select(x => x.Clone()).ToList();

            importedTypes.ForEach(importedType =>
            {
                if (!importedType.IsFirstSync)
                    importedType.StartID = currentElement.WorkItemsRange.Max + 1;
            });

            var projectsMapping = new ProjectsMappingHistoryElement()
            {
                Key = tfsProfile.ProjectsMapping[0].Key,
                Value = tfsProfile.ProjectsMapping[0].Value,
                WorkItemsRange = new CreatedWorkItemsRange() { Min = currentElement.WorkItemsRange.Max + 1, Max = -1 },
                IsCurrent = true,
                ImportedTypes = importedTypes
            };

            projectsMappingHistory.Add(projectsMapping);

            _profile.Get<ProjectsMappingHistory>().ReplaceWith(projectsMappingHistory);
        }
    }
}
