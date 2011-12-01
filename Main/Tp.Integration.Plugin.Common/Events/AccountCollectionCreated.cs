using System;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Events
{
	class AccountCollectionCreated : EventArgs
	{
		private readonly AccountCollection _accountCollection;

		public AccountCollectionCreated(AccountCollection accountCollection)
		{
			_accountCollection = accountCollection;
		}

		public AccountCollection AccountCollection1
		{
			get { return _accountCollection; }
		}
	}
}