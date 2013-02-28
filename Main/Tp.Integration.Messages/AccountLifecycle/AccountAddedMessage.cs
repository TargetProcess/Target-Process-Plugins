namespace Tp.Integration.Messages.AccountLifecycle
{
	using System;

	[Serializable]
	public sealed class AccountAddedMessage
		: IAccountLifecycleMessage
	{
		public AccountAddedMessage()
		{
		}

		public AccountAddedMessage(string accountName)
		{
			AccountName = accountName;
		}

		public string AccountName { get; set; }
	}
}
