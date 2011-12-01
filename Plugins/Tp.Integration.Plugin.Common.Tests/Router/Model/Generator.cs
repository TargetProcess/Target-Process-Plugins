using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class Generator<TMessage>
	{
		private readonly Func<int, TMessage> _createMessage;
		private readonly Func<TMessage> _createStopMessage;
		private readonly TimeSpan _period;
		private readonly int? _messagesCountToGenerate;
		private readonly int? _raiseErrorIndex;
		private int _currentIndex;

		public Generator(Func<int, TMessage> createMessage, Func<TMessage> createStopMessage, TimeSpan period, int? messagesCountToGenerate, int? raiseErrorIndex)
		{
			_createMessage = createMessage;
			_createStopMessage = createStopMessage;
			_period = period;
			_messagesCountToGenerate = messagesCountToGenerate;
			_raiseErrorIndex = raiseErrorIndex;
			_currentIndex = -1;
		}

		public TMessage Next()
		{
			Thread.Sleep(_period);
			int index = Interlocked.Increment(ref _currentIndex);
			if (index == _raiseErrorIndex)
			{
				throw new Exception();
			}
			TMessage message = (index == _messagesCountToGenerate)
			                   	? _createStopMessage() 
			                   	: _createMessage(index);
			return message;
		}
	}

	class Generator
	{
		public static IMessageSource<TestMessage> CreateSource(string sourceName, int messagesCountToGenerate, int? raiseErrorOnIndex)
		{
			var generator = new Generator<TestMessage>(i => TestMessage.NewMessage(sourceName, i.ToString()), () => TestMessage.StopMessage(sourceName), TimeSpan.FromSeconds(0.1), messagesCountToGenerate, raiseErrorOnIndex);
			Func<TestMessage> getNextMessage = generator.Next;
			IObservable<TestMessage> observable = Observable.Defer(Observable.FromAsyncPattern(getNextMessage.BeginInvoke, r => getNextMessage.EndInvoke(r)));
			return new MessageSource<TestMessage>(sourceName, observable);
		}

		public static IMessageSource<TestMessage> CreateSource(string sourceName, int sourcesCount, int messagesCountToGenerate, int? raiseErrorOnIndex)
		{
			List<IObservable<TestMessage>> observables = Enumerable.Range(0, sourcesCount).Select(i => CreateSource(i.ToString(), messagesCountToGenerate, raiseErrorOnIndex)).StaticCast(TypeOf<IObservable<TestMessage>>.Self).ToList();
			return new MessageSource<TestMessage>(sourceName, observables.Merge());
		}
	}
}