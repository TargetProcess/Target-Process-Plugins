// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus.Saga;

namespace Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Entities
{
    [Serializable]
    public class WorkItemEntity : ISagaEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public WorkItemInfo WorkItem { get; set; }
        public bool CreatingEntity { get; set; }
    }
}
