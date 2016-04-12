using System.Diagnostics;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	[DebuggerDisplay("AccountName =  {_accountName.Value}")]
	class AccountDomainObject : IAccount
	{
		private readonly AccountName _accountName;
		private readonly ProfileCollection _profileCollection;

		public AccountDomainObject(AccountName accountName, ProfileCollection profileCollection)
		{
			_accountName = accountName;
			_profileCollection = profileCollection;
		}

		public AccountDomainObject(AccountDomainObject other)
		{
			_accountName = other._accountName;
			_profileCollection = new ProfileCollection(other._profileCollection);
		}
		
		public AccountName Name
		{
			get { return _accountName; }
		}

		IProfileCollectionReadonly IAccountReadonly.Profiles
		{
			get { return _profileCollection; }
		}

		public IProfileCollection Profiles
		{
			get { return _profileCollection; }
		}
	}
}