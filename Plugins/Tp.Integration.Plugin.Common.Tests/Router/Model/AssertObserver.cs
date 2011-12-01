using System;
using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class AssertObserver : IObserver<TestMessage>
	{
		private readonly Action<IList<TestMessage>> _onComplete;
		private readonly List<TestMessage> _buffer;

		public AssertObserver(Action<IList<TestMessage>> onComplete)
		{
			_onComplete = onComplete;
			_buffer = new List<TestMessage>();
		}

		public void OnNext(TestMessage message)
		{
			Console.WriteLine(message.ToString());
			lock(_buffer)
			{
				_buffer.Add(message);
			}
		}

		public void OnError(Exception error)
		{
			Console.WriteLine(error.ToString());
		}

		public void OnCompleted()
		{
			try
			{
				List<TestMessage> copy;
				lock (_buffer)
				{
					copy = new List<TestMessage>(_buffer);
				}
				_onComplete(copy.AsReadOnly());
			}
			catch(Exception e)
			{
				OnError(e);
			}
		}
	}
}