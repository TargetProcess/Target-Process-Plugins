// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Storage.Repositories
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IProfileFactory _profileFactory;
        private readonly IAccountPersister _accountPersister;
        private readonly IEventAggregator _eventAggregator;

        public AccountRepository(IProfileRepository profileRepository, IProfileFactory profileFactory, IAccountPersister accountPersister,
            IEventAggregator eventAggregator)
        {
            _profileRepository = profileRepository;
            _profileFactory = profileFactory;
            _accountPersister = accountPersister;
            _eventAggregator = eventAggregator;
        }

        public IList<AccountDomainObject> GetAll()
        {
            return _accountPersister.GetAll()
                .Select(CreateAccount)
                .ToList()
                .AsReadOnly();
        }

        public AccountDomainObject Add(AccountName accountName)
        {
            var newAccount = _accountPersister.Add(accountName);
            return CreateAccount(newAccount.Name, Enumerable.Empty<Profile>());
        }

        public AccountDomainObject GetBy(AccountName accountName)
        {
            var account = _accountPersister.GetBy(accountName);
            if (account == null)
            {
                return null;
            }

            return CreateAccount(account);
        }

        public void Remove(AccountName accountName)
        {
            _accountPersister.Remove(accountName);
        }

        private AccountDomainObject CreateAccount(Account account)
        {
            return CreateAccount(account.Name, account.Profiles);
        }

        private AccountDomainObject CreateAccount(AccountName accountName, IEnumerable<Profile> profiles)
        {
            var profileDomainObjects = profiles.Select(p => _profileFactory.Create(p, accountName)).ToList();
            var profileCollection = new ProfileCollection(accountName, profileDomainObjects)
            {
                EventAggregator = _eventAggregator,
                ProfileRepository = _profileRepository
            };
            return new AccountDomainObject(accountName, profileCollection);
        }
    }
}
