using System;

namespace Tp.Integration.Messages.AccountLifecycle
{
	[Serializable]
	public sealed class AccountRemovedMessage
		: IAccountLifecycleMessage
	{
		public AccountRemovedMessage()
		{
		}

		public AccountRemovedMessage(string accountName)
		{
			AccountName = accountName;
		}

		public string AccountName { get; set; }
	}
}
