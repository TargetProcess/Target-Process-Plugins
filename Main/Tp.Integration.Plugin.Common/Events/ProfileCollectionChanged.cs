using System;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Events
{
	class ProfileCollectionChanged : EventArgs
	{
		private readonly IProfileCollectionReadonly _profileCollection;
		private readonly AccountName _accountName;

		public ProfileCollectionChanged(IProfileCollectionReadonly profileCollection, AccountName accountName)
		{
			_profileCollection = profileCollection;
			_accountName = accountName;
		}

		public AccountName AccountName
		{
			get { return _accountName; }
		}

		public IProfileCollectionReadonly ProfileCollection
		{
			get { return _profileCollection; }
		}
	}
}