namespace Tp.Integration.Plugin.Common.Domain
{
    class AccountDomainObjectVersion
    {
        private readonly AccountDomainObject _account;
        private readonly AccountDomainObjectVersion _parent;

        private AccountDomainObjectVersion(AccountDomainObject account, AccountDomainObjectVersion parent = null)
        {
            _account = account;
            _parent = parent;
        }

        public AccountDomainObjectVersion CreateChildVersion()
        {
            return new AccountDomainObjectVersion(new AccountDomainObject(_account), this);
        }

        public AccountDomainObject Account
        {
            get { return _account; }
        }

        public bool IsChildOf(AccountDomainObjectVersion maybeParent)
        {
            return _parent == maybeParent;
        }

        public static AccountDomainObjectVersion Root(AccountDomainObject account)
        {
            return new AccountDomainObjectVersion(account);
        }
    }
}
