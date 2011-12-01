// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Testing.Common.Persisters
{
	public class AccountInMemoryPersister : IAccountPersister
	{
		private readonly List<Account> _accounts = new List<Account>();

		public IList<Account> GetAll()
		{
			_accounts.ForEach(LoadProfiles);
			return _accounts;
		}

		private void LoadProfiles(Account account)
		{
			account.Profiles.Clear();
			account.Profiles.AddRange(ObjectFactory.GetInstance<IProfilePersister>().GetAll(account.Name));
		}

		public Account Add(AccountName accountName)
		{
			var account = new Account {Name = accountName.Value};
			_accounts.Add(account);
			return account;
		}

		public Account GetBy(AccountName accountName)
		{
			var account = _accounts.FirstOrDefault(x => x.Name == accountName);
			LoadProfiles(account);
			return account;
		}
	}
}