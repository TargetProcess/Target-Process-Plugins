using System;

namespace Tp.Core
{
    public interface ITransactionScope : ILockOwner, IDisposable
    {
        void Commit();
    }

    public class NoTransactionScope : ITransactionScope
    {
        public static readonly ITransactionScope Instance = new NoTransactionScope();
        private NoTransactionScope() { }
        public bool IsLockTaken(ILock _)
        {
            return false;
        }

        public void Dispose()
        {
        }

        public void Commit()
        {
        }
    }
}
