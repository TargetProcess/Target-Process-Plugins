// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.PopEmailIntegration.EmailReader
{
    public class MessagePackSize : IMessagePackSize
    {
        public MessagePackSize(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }

    public interface IMessagePackSize
    {
        int Value { get; }
    }
}
