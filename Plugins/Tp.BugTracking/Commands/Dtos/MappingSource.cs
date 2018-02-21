// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.BugTracking.Commands.Dtos
{
    [DataContract]
    public class MappingSource
    {
        [DataMember]
        public MappingSourceEntry States { get; set; }

        [DataMember]
        public MappingSourceEntry Priorities { get; set; }

        [DataMember]
        public MappingSourceEntry Severities { get; set; }

        [DataMember]
        public MappingSourceEntry Roles { get; set; }
    }
}
