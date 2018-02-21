// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.Tfs.WorkItemsIntegration
{
    [DataContract]
    public class SimpleMappingElement
    {
        [DataMember]
        public string First { get; set; }

        [DataMember]
        public string Second { get; set; }

        public bool IsEmptyValue()
        {
            return string.IsNullOrEmpty(Second);
        }
    }
}
