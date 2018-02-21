using System;
using System.Collections;
using System.Collections.Generic;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
    public class MessageSource<TMessage> : IMessageSource<TMessage>
    {
        private readonly string _name;
        private readonly IEnumerable<IObservable<TMessage>> _observables;

        public MessageSource(string name, IEnumerable<IObservable<TMessage>> observables)
        {
            _name = name;
            _observables = observables;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerator<IObservable<TMessage>> GetEnumerator()
        {
            return _observables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
