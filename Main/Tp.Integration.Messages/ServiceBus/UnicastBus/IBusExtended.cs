using NServiceBus;
using NServiceBus.Unicast;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public interface IBusExtended : IUnicastBus
    {
        void SendLocalUi(params IMessage[] message);
        void CleanupOutgoingHeaders();
    }
}
