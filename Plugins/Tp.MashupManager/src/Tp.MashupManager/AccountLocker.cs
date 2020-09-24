using System;
using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.MashupManager
{
    public class AccountLocker : IAccountLocker
    {
        private readonly Locker _locker = new Locker();

        private readonly int _timeout = MashupManagerSettings.AccountLockAcquiringTimeoutMs;

        public T ExecuteInAccountLock<T>(Func<T> operation, AccountName account)
        {
            return _locker
                .TryAcquireLockAndExecute(account.Value, operation, _timeout)
                .GetOrThrow(() => new TimeoutException($"Failed to obtain account lock for {_timeout} ms"));
        }
    }
}
