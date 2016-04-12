using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;

namespace Tp.Integration.Plugin.Common
{
	public interface ITargetProcessConditionalMessageRouter
	{
		bool Handle(MessageEx message);
	}
}