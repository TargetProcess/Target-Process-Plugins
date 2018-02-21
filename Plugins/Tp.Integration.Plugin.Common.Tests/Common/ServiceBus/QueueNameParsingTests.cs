// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus.Unicast;
using NUnit.Framework;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.ServiceBus
{
    [TestFixture]
    [Category("PartPlugins1")]
    internal class QueueNameParsingTests
    {
        [Test]
        public void PopulateUiQueueFromFullQueueName()
        {
            UnicastBus.GetUiQueueName("input@truhtanov")
                .Should(Be.EqualTo("inputUI@truhtanov"),
                    "UnicastBus.GetUiQueueName(\"input@truhtanov\").Should(Be.EqualTo(\"inputUI@truhtanov\"))");
            UnicastBus.GetUiQueueName("input")
                .Should(Be.EqualTo("inputUI"), "UnicastBus.GetUiQueueName(\"input\").Should(Be.EqualTo(\"inputUI\"))");
        }
    }
}
