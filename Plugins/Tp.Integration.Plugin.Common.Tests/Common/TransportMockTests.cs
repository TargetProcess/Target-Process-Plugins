// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Reflection;
using NServiceBus;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class TransportMockTests : IHandleMessages<ReplyMessage>
    {
        private TransportMock _transport;
        private static bool _handled;
        private static ReplyMessage _replyMessage;

        [SetUp]
        public void Init()
        {
            _handled = false;
            _transport = TransportMock.CreateWithoutStructureMapClear(typeof(ReplyMessage).Assembly, Assembly.GetExecutingAssembly());
            _transport.AddProfile("Profile_1");
        }

        [Test]
        public void ShouldAutoreplyOnAMessage()
        {
            //Pre-Conditions
            _transport.On<RequestCreatedMessage>().Reply(
                incomingMessage => new ReplyMessage());

            //Action

            var tpBus = ObjectFactory.GetInstance<ITpBus>();
            var sagaId = Guid.NewGuid();
            tpBus.SetOutSagaId(sagaId);
            tpBus.Send(new RequestCreatedMessage());
            _transport.RaiseTransportMessageReceived();

            //Post-Conditions
            _handled.Should(Be.True, "_handled.Should(Be.True)");
            _replyMessage.SagaId.Should(Be.EqualTo(sagaId), "_replyMessage.SagaId.Should(Be.EqualTo(sagaId))");
        }

        [Test]
        public void ShouldAutoreplyOnAMessageWithCondition()
        {
            //Pre-Conditions
            const string requestName = "My Name";
            _transport.On<RequestCreatedMessage>(x => x.Dto.Name == requestName).Reply(
                x => new ReplyMessage());

            //Action
            ObjectFactory.GetInstance<ITpBus>().Send(new RequestCreatedMessage { Dto = new RequestDTO { Name = requestName } });
            _transport.RaiseTransportMessageReceived();

            //Post-Conditions
            _handled.Should(Be.True, "_handled.Should(Be.True)");
        }

        [Test]
        public void ShouldAutoreplyOnAMessageWhenDoesNotMeetCondition()
        {
            //Pre-Conditions
            const string requestName = "My Name";
            _transport.On<RequestCreatedMessage>(x => false).Reply(
                x => new ReplyMessage());

            //Action
            ObjectFactory.GetInstance<ITpBus>().Send(new RequestCreatedMessage { Dto = new RequestDTO { Name = requestName } });
            _transport.RaiseTransportMessageReceived();

            //Post-Conditions
            _handled.Should(Be.False, "_handled.Should(Be.False)");
        }

        [Test]
        public void ShouldSerializeMessagesPassedToTransport()
        {
            //Pre-Conditions
            var message = new RequestCreatedMessage { Dto = new RequestDTO { Name = "My Name" } };
            //Action
            ObjectFactory.GetInstance<ITpBus>().Send(message);
            _transport.RaiseTransportMessageReceived();

            //Post-Conditions
            var tpMessage = _transport.TpQueue.GetMessages<RequestCreatedMessage>().Single();
            tpMessage.Should(Be.Not.EqualTo(message), "tpMessage.Should(Be.Not.EqualTo(message))");
        }

        [Test]
        public void ShouldKeepMessagesFromTpInLocalQueue()
        {
            //Pre-Conditions
            const string requestName = "My Name";
            _transport.On<RequestCreatedMessage>(x => true).Reply(
                x => new ReplyMessage());

            //Action
            ObjectFactory.GetInstance<ITpBus>().Send(new RequestCreatedMessage { Dto = new RequestDTO { Name = requestName } });
            _transport.RaiseTransportMessageReceived();

            //Post-Conditions
            _transport.LocalQueue.GetMessages<ReplyMessage>()
                .Should(Be.Not.Empty, "_transport.LocalQueue.GetMessages<ReplyMessage>().Should(Be.Not.Empty)");
        }

        public void Handle(ReplyMessage message)
        {
            _handled = true;
            _replyMessage = message;
        }
    }

    public class ReplyMessage : ISagaMessage
    {
        public Guid SagaId { get; set; }
    }
}
