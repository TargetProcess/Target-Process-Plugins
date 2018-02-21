﻿// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using NServiceBus.Unicast.Transport;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport;

namespace Tp.Integration.Testing.Common
{
    public static class TransportMockConfig
    {
        public static Configure TransportMock(this Configure config)
        {
            ObjectFactory.Configure(x =>
            {
                x.For<TransportMock>().HybridHttpOrThreadLocalScoped().Use<TransportMock>();
                x.Forward<TransportMock, ITransport>();
                x.Forward<TransportMock, IMsmqTransport>();
                x.FillAllPropertiesOfType<ITransport>();
                x.FillAllPropertiesOfType<IMsmqTransport>();
            });
            return config;
        }
    }
}
