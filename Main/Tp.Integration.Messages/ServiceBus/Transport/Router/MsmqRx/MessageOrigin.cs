namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public struct MessageOrigin
	{
		public string FormatName { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
	}
}