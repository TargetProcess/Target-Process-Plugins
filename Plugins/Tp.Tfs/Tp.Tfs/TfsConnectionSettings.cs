// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.Settings;
using Tp.Tfs.WorkItemsIntegration;

namespace Tp.Tfs
{
    [DataContract, KnownType(typeof(ConnectionSettings))]
    public class TfsConnectionSettings : ConnectionSettings
    {
        [DataMember]
        public SimpleMappingContainer EntityMapping { get; set; }

        [DataMember]
        public MappingContainer ProjectsMapping { get; set; }

        [DataMember]
        public bool SourceControlEnabled { get; set; }

        [DataMember]
        public bool WorkItemsEnabled { get; set; }

        [DataMember]
        public string StartWorkItem { get; set; }
    }
}
