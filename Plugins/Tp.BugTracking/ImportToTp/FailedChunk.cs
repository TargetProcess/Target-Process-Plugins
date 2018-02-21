// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.BugTracking.ImportToTp
{
    [Serializable]
    public class FailedChunk
    {
        public FailedChunk()
        {
        }

        public FailedChunk(int[] chunk)
        {
            Chunk = chunk;
        }

        public int[] Chunk { get; set; }
    }
}
