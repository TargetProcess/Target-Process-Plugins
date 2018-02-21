using System;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
    class TestMessage : IMessageWithId
    {
        private readonly string _tag;
        private readonly string _body;

        private TestMessage(string id, string tag, string body)
        {
            Id = id;
            _tag = tag;
            _body = body;
        }

        public string Tag
        {
            get { return _tag; }
        }

        public string Body
        {
            get { return _body; }
        }

        public override string ToString()
        {
            return string.Format("Tag:{0}, Body:{1}", Tag, Body);
        }

        public static TestMessage NewMessage(string tag, string body)
        {
            return new TestMessage(Guid.NewGuid().ToString(), tag, body);
        }

        public static TestMessage StopMessage(string tag)
        {
            return NewMessage(tag, "stop");
        }

        public static bool IsNotStopMessage(string tag, TestMessage message)
        {
            return message.Tag != tag || message.Body != "stop";
        }

        public string Id { get; }
    }
}
