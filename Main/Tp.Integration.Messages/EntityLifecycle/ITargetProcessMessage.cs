using NServiceBus;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Marker interface to indicate that a class represents a message that can be sent to TargetProcess or retrieved from TargetProcess.
	/// </summary>
	public interface ITargetProcessMessage : IMessage
	{
	}
}
