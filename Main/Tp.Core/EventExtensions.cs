using System;

namespace Tp.Core
{
    public static class EventExtensions
    {
        public static void Raise(this EventHandler handler, object sender, EventArgs args)
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs args)
            where TEventArgs : EventArgs
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public static void Raise(this EventHandler handler, object sender)
        {
            Raise(handler, sender, EventArgs.Empty);
        }
    }
}
