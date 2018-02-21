// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.Common.Mapping
{
    [DataContract]
    public class Mappings
    {
        [DataMember]
        public MappingContainer States { get; set; }

        [DataMember]
        public MappingContainer Severities { get; set; }

        [DataMember]
        public MappingContainer Priorities { get; set; }
    }

    public class MappingContainer : List<MappingElement>
    {
        public MappingLookup this[string key]
        {
            get
            {
                var mappingElement = Find(x => x.Key.ToLower().Trim() == key.ToLower().Trim());
                return mappingElement?.Value;
            }
        }
    }
}
