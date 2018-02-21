// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Tfs.WorkItemsIntegration.ChangedFieldsPolicy;
using Tp.Tfs.WorkItemsIntegration.FieldsMapping;

namespace Tp.Tfs.WorkItemsIntegration.ChangedFieldsMapping
{
    public abstract class ChangedFieldsPolicyBase : IChangedFieldsMappingPolicy
    {
        public Dictionary<string, Enum> FieldsMap { get; set; }

        public Enum WorkItemFieldToTpField(string workItemType, string workItemfield)
        {
            string tpField = WorkItemsFieldsMapper.Instance.GetMappedTpField(workItemType, workItemfield);
            return FieldsMap[tpField];
        }
    }
}
