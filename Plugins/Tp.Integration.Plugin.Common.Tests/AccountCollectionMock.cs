using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Tests
{
    public class AccountCollectionMock
        : IAccountCollection
    {
        private readonly Dictionary<AccountName, IAccount> _accounts = new Dictionary<AccountName, IAccount>();

        public Dictionary<AccountName, IAccount> Accounts
        {
            get { return _accounts; }
        }

        public IEnumerator<IAccountReadonly> GetEnumerator()
        {
            return _accounts.Values.Cast<IAccountReadonly>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IAccount GetOrCreate(AccountName accountName)
        {
            IAccount account;
            if (_accounts.TryGetValue(accountName, out account))
            {
                return account;
            }

            return
                _accounts[accountName] =
                    new AccountDomainObject(accountName, new ProfileCollection(accountName, new ProfileDomainObject[0]));
        }

        public void Remove(AccountName accountName)
        {
            _accounts.Remove(accountName);
        }
    }
}
