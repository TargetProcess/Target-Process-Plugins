using System;
using Tp.Integration.Messages;

namespace Tp.MashupManager
{
    public interface IAccountLocker
    {
        T ExecuteInAccountLock<T>(Func<T> operation, AccountName account);
    }
}