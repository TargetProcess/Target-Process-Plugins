// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.Git.VersionControlSystem
{
    static class DictionaryExtensions
    {
        public static U Get<T, U>(this IDictionary<T, U> d, T key)
        {
            U val;
            d.TryGetValue(key, out val);
            return val;
        }

        public static U Put<T, U>(this IDictionary<T, U> d, T key, U value)
        {
            U old;
            d.TryGetValue(key, out old);
            d[key] = value;
            return old;
        }
    }
}
