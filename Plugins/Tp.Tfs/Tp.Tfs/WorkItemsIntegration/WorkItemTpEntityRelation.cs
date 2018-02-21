// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.Tfs.WorkItemsIntegration
{
    [Serializable]
    [DataContract]
    public class WorkItemTpEntityRelation
    {
        public WorkItemTpEntityRelation()
        {
        }

        public WorkItemTpEntityRelation(WorkItemId workItemId, TpEntityId tpEntityId)
        {
            WorkItemId = workItemId;
            TpEntityId = tpEntityId;
        }

        [DataMember]
        public WorkItemId WorkItemId { get; set; }

        [DataMember]
        public TpEntityId TpEntityId { get; set; }
    }
}
