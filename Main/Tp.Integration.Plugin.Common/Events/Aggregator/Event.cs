using System;

namespace Tp.Integration.Plugin.Common.Events
{
    using Event = EventArgs;

    class Event<TEvent> : EventBase where TEvent : EventArgs
    {
        public IDisposable Subscribe(Action<TEvent> handler)
        {
            Action<EventArgs> coreHandler = e => handler((TEvent) e);
            return base.Subscribe(coreHandler);
        }

        public void Raise(TEvent e)
        {
            Event coreEvent = e;
            Raise(coreEvent);
        }
    }
}
