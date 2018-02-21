// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Plugin.Core.Attachments
{
    public interface IBufferSize
    {
        int Value { get; }
    }

    public class BufferSize : IBufferSize
    {
        public BufferSize(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}
