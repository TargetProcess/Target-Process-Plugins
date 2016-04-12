namespace Tp.Integration.Common
{
	/// <summary>
	/// Describes the type of the Message
	/// </summary>
	public enum MessageTypeEnum
	{
		/// <summary>
		/// Undefined
		/// </summary>
		None = 0,

		/// <summary>
		/// Inbox
		/// </summary>
		Inbox = 1,

		/// <summary>
		/// Outbox
		/// </summary>
		Outbox = 2,

		/// <summary>
		/// Public 
		/// </summary>
		Public = 3
	}
}
