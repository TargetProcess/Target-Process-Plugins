// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl;
using Tp.Tfs.WorkItemsIntegration;

namespace Tp.Tfs
{
    public class TfsCurrentProfileToConnectionSettingsAdapter : CurrentProfileToConnectionSettingsAdapter<TfsPluginProfile>
    {
        public TfsCurrentProfileToConnectionSettingsAdapter(IStorageRepository repository)
            : base(repository)
        {
        }

        public MappingContainer ProjectsMapping
        {
            get => Profile.ProjectsMapping;
            set => Profile.ProjectsMapping = value;
        }

        public SimpleMappingContainer EntityMapping
        {
            get => Profile.EntityMapping;
            set => Profile.EntityMapping = value;
        }

        public bool WorkItemsEnabled
        {
            get => Profile.WorkItemsEnabled;
            set => Profile.WorkItemsEnabled = value;
        }
    }
}
