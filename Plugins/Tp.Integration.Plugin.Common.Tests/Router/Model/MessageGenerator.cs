using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class MessageGenerator<TMessage>
	{
		private readonly Func<int, TMessage> _createMessage;
		private readonly Func<TMessage> _createStopMessage;
		private readonly TimeSpan _interval;
		private readonly int? _messagesCount;
		private readonly int? _raiseErrorIndex;
		private int _currentIndex;

		public MessageGenerator(Func<int, TMessage> createMessage, Func<TMessage> createStopMessage, TimeSpan interval, int? messagesCount, int? raiseErrorIndex)
		{
			_createMessage = createMessage;
			_createStopMessage = createStopMessage;
			_interval = interval;
			_messagesCount = messagesCount;
			_raiseErrorIndex = raiseErrorIndex;
			_currentIndex = -1;
		}

		public TMessage Next()
		{
			Thread.Sleep(_interval);
			int index = Interlocked.Increment(ref _currentIndex);
			if (index == _raiseErrorIndex)
			{
				throw new Exception();
			}
			return index == _messagesCount ? _createStopMessage() : _createMessage(index);
		}
	}

	class MessageGenerator
	{
		public static IMessageSource<TestMessage> CreateSource(string sourceName, int messagesCountToGenerate, int? raiseErrorOnIndex = null, bool swallowException = false)
		{
			Func<IObservable<TestMessage>> messageGenerator = CreateMessageGenerator(sourceName, messagesCountToGenerate, TimeSpan.FromSeconds(0.1), raiseErrorOnIndex);
			return new MessageSource<TestMessage>(sourceName, GetMessageStream(messageGenerator, swallowException));
		}

		public static IMessageSource<TestMessage> CreateCompositeSource(string sourceName, int sourcesCount, int messagesCountToGenerate, int? raiseErrorOnIndex = null, bool swallowException = false)
		{
			List<IObservable<TestMessage>> complexMessageGenerator = Enumerable.Range(0, sourcesCount)
																				.Select(i => CreateMessageGenerator(i.ToString(), messagesCountToGenerate, TimeSpan.FromSeconds(0.1), raiseErrorOnIndex))
																				.Select(f => Observable.Defer(f).Repeat(messagesCountToGenerate + 1))
																				.Merge()
																				.ToEnumerable()
																				.Select(Observable.Return)
																				.ToList();
			return new MessageSource<TestMessage>(sourceName, new MessageSource<TestMessage>(sourceName, complexMessageGenerator));
		}
		
		private static Func<IObservable<TestMessage>> CreateMessageGenerator(string sourceName, int messagesCountToGenerate, TimeSpan interval, int? raiseErrorOnIndex)
		{
			var generator = new MessageGenerator<TestMessage>(i => TestMessage.NewMessage(sourceName, i.ToString()), () => TestMessage.StopMessage(sourceName), interval, messagesCountToGenerate, raiseErrorOnIndex);
			Func<TestMessage> getNextMessage = generator.Next;
			return Observable.FromAsyncPattern(getNextMessage.BeginInvoke, r => getNextMessage.EndInvoke(r));
		}

		private static IEnumerable<IObservable<TestMessage>> GetMessageStream(Func<IObservable<TestMessage>> observableFactory, bool swallowException)
		{
			while(true)
			{
				IObservable<TestMessage> v = observableFactory().Catch((Exception e) =>
				                                          	{
				                                          		if (swallowException)
				                                          		{
				                                          			Console.WriteLine(e);
				                                          		}
				                                          		else
				                                          		{
				                                          			throw e;
				                                          		}
				                                          		return observableFactory();
				                                          	});
				yield return v;
			}
		}
	}
}