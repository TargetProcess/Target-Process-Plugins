using System.Reactive.Subjects;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class TestMessageProducer : IMessageProducer<TestMessage>
	{
		private readonly ISubject<TestMessage> _subject;

		public TestMessageProducer(ISubject<TestMessage> subject)
		{
			_subject = subject;
		}

		public void Produce(TestMessage message)
		{
			_subject.OnNext(message);
		}

		public string Name
		{
			get { return string.Empty; }
		}

		public void Dispose()
		{
		}
	}
}