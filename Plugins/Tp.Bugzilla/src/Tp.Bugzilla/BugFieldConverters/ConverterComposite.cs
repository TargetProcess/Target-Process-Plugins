// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using StructureMap;
using Tp.BugTracking.BugFieldConverters;

namespace Tp.Bugzilla.BugFieldConverters
{
    public class ConverterComposite : ConverterCompositeBase<BugzillaBug>
    {
        protected override IEnumerable<IBugConverter<BugzillaBug>> Converters
        {
            get
            {
                return new List<IBugConverter<BugzillaBug>>
                {
                    ObjectFactory.GetInstance<NameConverter>(),
                    ObjectFactory.GetInstance<DescriptionConverter>(),
                    ObjectFactory.GetInstance<EntityStateConverter>(),
                    ObjectFactory.GetInstance<SeverityConverter>(),
                    ObjectFactory.GetInstance<PriorityConverter>(),
                    ObjectFactory.GetInstance<CreateDateConverter>(),
                    ObjectFactory.GetInstance<CommentConverter>(),
                    ObjectFactory.GetInstance<OwnerConverter>(),
                };
            }
        }
    }
}
