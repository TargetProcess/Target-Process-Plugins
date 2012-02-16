// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Events;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Storage.Repositories;

namespace Tp.Integration.Plugin.Common.Domain
{
	[DebuggerDisplay("AccountsCount = {_accountLatestVersions.Count}")]
	class AccountCollection : IAccountCollection
	{
		private readonly IAccountRepository _accountRepository;
		private readonly object _gate;
		private Dictionary<AccountName, AccountDomainObjectVersion> _accountLatestVersions;
		[ThreadStatic]
		private static AccountDomainObjectVersion _accountCurrentVersion;

		public AccountCollection(IAccountRepository accountRepository, IEventAggregator eventAggregator)
		{
			_accountRepository = accountRepository;
			_gate = new object();
			eventAggregator.Get<Event<ProfileChanged>>().Subscribe(e => OnAccountChanged(e.AccountName));
			eventAggregator.Get<Event<ProfileCollectionChanged>>().Subscribe(e => OnAccountChanged(e.AccountName));
			eventAggregator.Get<Event<AccountCollectionCreated>>().Raise(new AccountCollectionCreated(this));
		}

		public IAccount GetOrCreate(AccountName accountName)
		{
			lock (_gate)
			{
				AccountDomainObjectVersion accountLatestVersion;
				if (Accounts.TryGetValue(accountName, out accountLatestVersion))
				{
					if (_accountCurrentVersion != null && _accountCurrentVersion.IsChildOf(accountLatestVersion))
					{
						return _accountCurrentVersion.Account;
					}
					_accountCurrentVersion = accountLatestVersion.CreateChildVersion();
				}
				else
				{
					AccountDomainObject newAccount = _accountRepository.Add(accountName);
					var newAccountVersion = AccountDomainObjectVersion.Root(newAccount);
					Accounts.Add(accountName, newAccountVersion);
					_accountCurrentVersion = newAccountVersion.CreateChildVersion();
				}
			}
			return _accountCurrentVersion.Account;
		}

		public IEnumerator<IAccountReadonly> GetEnumerator()
		{
			return CreateSnapshot().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		private IEnumerable<IAccountReadonly> CreateSnapshot()
		{
			lock (_gate)
			{
				return Accounts.Values.Select(acc => acc.CreateChildVersion().Account)
										.ToList();
			}
		}

		private void OnAccountChanged(AccountName accountName)
		{
			AccountDomainObject account = _accountRepository.GetBy(accountName);
			lock (_gate)
			{
				Accounts[accountName] = AccountDomainObjectVersion.Root(account);
			}
		}

		private Dictionary<AccountName, AccountDomainObjectVersion> Accounts
		{
			get { return _accountLatestVersions ?? (_accountLatestVersions = _accountRepository.GetAll().ToDictionary(acc => acc.Name, AccountDomainObjectVersion.Root)); }
		}
	}
}