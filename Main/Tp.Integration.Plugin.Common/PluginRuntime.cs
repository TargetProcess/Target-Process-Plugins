using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Events;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common
{
	class PluginRuntime
	{
		private readonly IPluginPersister _pluginPersister;
		private readonly IEventAggregator _eventAggregator;
		private readonly ITpBus _bus;

		public PluginRuntime(IPluginPersister pluginPersister, EventAggregator eventAggregator, ITpBus bus)
		{
			_pluginPersister = pluginPersister;
			_eventAggregator = eventAggregator;
			_bus = bus;
			_eventAggregator.Get<Event<AccountCollectionCreated>>().Subscribe(OnAccountCollectionCreated);
			_eventAggregator.Get<Event<ProfileChanged>>().Subscribe(OnProfileChanged);
		}

		public IEventAggregator EventAggregator
		{
			get { return _eventAggregator; }
		}

		private void OnAccountCollectionCreated(AccountCollectionCreated _)
		{
			_pluginPersister.CreateIfMissing();
		}

		private void OnProfileChanged(ProfileChanged e)
		{
			if (!e.Profile.Initialized)
			{
				return;
			}
			IStorage<ITargetProcessMessage> storage = e.Profile.Get<ITargetProcessMessage>();
			foreach (ITargetProcessMessage message in storage)
			{
				_bus.SendLocalWithContext(e.Profile.Name, e.AccountName, message);
			}
			storage.Clear();
		}
	}
}