// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.BugTracking.BugFieldConverters
{
    public abstract class ConverterCompositeBase<T> : IBugConverter<T>
    {
        protected abstract IEnumerable<IBugConverter<T>> Converters { get; }

        public void Apply(T thirdPartyBug, ConvertedBug convertedBug)
        {
            foreach (var converter in Converters)
            {
                converter.Apply(thirdPartyBug, convertedBug);
            }
        }
    }
}
