// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;

namespace Tp.Tfs.WorkItemsIntegration.ChangedFieldsPolicy
{
    public interface IChangedFieldsMappingPolicy
    {
        Dictionary<string, Enum> FieldsMap { get; set; }
        Enum WorkItemFieldToTpField(string workItemType, string workItemfield);
    }
}
