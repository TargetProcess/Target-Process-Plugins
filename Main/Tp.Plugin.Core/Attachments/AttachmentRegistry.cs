// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap.Configuration.DSL;

namespace Tp.Plugin.Core.Attachments
{
    public class AttachmentRegistry : Registry
    {
        public AttachmentRegistry()
        {
            For<IBufferSize>().Singleton().Use(new BufferSize(1000000));
        }
    }
}
