using System;
using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common.Events.Aggregator
{
	class EventAggregator : IEventAggregator
	{
		private readonly Dictionary<Type, EventBase> _events;

		public EventAggregator()
		{
			_events = new Dictionary<Type, EventBase>();
		}

		public TEvent Get<TEvent>() where TEvent : EventBase, new()
		{
			lock (_events)
			{
				EventBase existingEvent;
				if (!_events.TryGetValue(typeof (TEvent), out existingEvent))
				{
					var newEvent = new TEvent();
					_events[typeof (TEvent)] = newEvent;
					return newEvent;
				}
				return (TEvent) existingEvent;
			}
		}
	}
}