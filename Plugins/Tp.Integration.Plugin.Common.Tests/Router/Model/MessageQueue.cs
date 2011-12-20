using System.Collections.Generic;
using System.Threading;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class MessageQueue<T>
	{
		private readonly Queue<T> _queue;
		public MessageQueue()
		{
			_queue = new Queue<T>();
		}

		public void Enqueue(T message)
		{
			lock (_queue)
			{
				bool wasEmpty = _queue.Count == 0;
				_queue.Enqueue(message);
				if (wasEmpty)
				{
					Monitor.Pulse(_queue);
				}
			}
		}

		public T Dequeue()
		{
			lock (_queue)
			{
				while(_queue.Count == 0)
				{
					Monitor.Wait(_queue);
				}
				return _queue.Dequeue();
			}
		}
	}
}