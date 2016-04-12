using NServiceBus;

namespace Tp.Integration.Messages.PluginLifecycle
{
	/// <summary>
	/// Marker interface to indicate that a class represents plugin local message.
	/// </summary>
	public interface IPluginLocalMessage : IMessage
	{
	}
}
