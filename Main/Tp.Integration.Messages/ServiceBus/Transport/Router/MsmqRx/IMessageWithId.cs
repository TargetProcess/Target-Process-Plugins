namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
    public interface IMessageWithId
    {
        string Id { get; }
    }
}
