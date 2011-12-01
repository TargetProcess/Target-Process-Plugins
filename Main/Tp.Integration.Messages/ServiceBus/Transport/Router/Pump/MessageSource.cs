using System;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
	public class MessageSource<TMessage> : IMessageSource<TMessage>
	{
		private readonly string _name;
		private readonly IObservable<TMessage> _observable;

		public MessageSource(string name, IObservable<TMessage> observable)
		{
			_name = name;
			_observable = observable;
		}

		public string Name { get { return _name; } }

		public IDisposable Subscribe(IObserver<TMessage> observer)
		{
			return _observable.Subscribe(observer);
		}
	}
}