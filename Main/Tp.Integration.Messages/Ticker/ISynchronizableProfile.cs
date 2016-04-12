namespace Tp.Integration.Messages.Ticker
{
	/// <summary>
	/// Inherit your profile from this interface to make it perform some repeated actions.
	/// </summary>
	public interface ISynchronizableProfile
	{
		/// <summary>
		/// Number of minutes after which you will receive <see cref="TickMessage"/>.
		/// Perform your logic in <see cref="TickMessage"/> handler.
		/// </summary>
		int SynchronizationInterval { get; }
	}
}
