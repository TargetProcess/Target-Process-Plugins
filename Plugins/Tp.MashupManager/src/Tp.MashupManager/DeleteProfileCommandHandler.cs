using NServiceBus;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.MashupManager
{
	public class DeleteProfileCommandHandler : IHandleMessages<ExecutePluginCommandCommand>
	{
		private readonly IBus _bus;

		public DeleteProfileCommandHandler(IBus bus)
		{
			_bus = bus;
		}

		public void Handle(ExecutePluginCommandCommand message)
		{
			if(message.CommandName == EmbeddedPluginCommands.DeleteProfile)
			{
				_bus.DoNotContinueDispatchingCurrentMessageToHandlers();
			}
		}
	}
}