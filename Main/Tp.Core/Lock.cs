using System.Threading;

namespace Tp.Core
{
    public interface ILock
    {
        void Acquire();
        void Release();
    }

    public class Lock : ILock
    {
        private readonly object _gate;

        public Lock(object gate)
        {
            _gate = gate;
        }

        public void Acquire()
        {
            Monitor.Enter(_gate);
        }

        public void Release()
        {
            Monitor.Exit(_gate);
        }
    }

    public class NoLock : ILock
    {
        public static readonly NoLock Instance = new NoLock();

        private NoLock()
        {
        }

        public void Acquire()
        {
        }

        public void Release()
        {
        }
    }

    public interface ILockOwner
    {
        bool IsLockTaken(ILock @lock);
    }
}
