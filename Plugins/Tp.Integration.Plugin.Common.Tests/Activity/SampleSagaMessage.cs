// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
    public class SampleSagaMessage : ITargetProcessMessage
    {
        public Guid SagaId { get; set; }
    }

    public class SampleSagaMessageHandler : IHandleMessages<SampleSagaMessage>
    {
        public void Handle(SampleSagaMessage message)
        {
            throw new Exception("test exception to check unhandled exception logging");
        }
    }
}
