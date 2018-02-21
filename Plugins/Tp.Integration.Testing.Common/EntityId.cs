// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Testing.Common
{
    public class EntityId
    {
        private static int _nextUserId;

        public static int Next()
        {
            return ++_nextUserId;
        }
    }
}
