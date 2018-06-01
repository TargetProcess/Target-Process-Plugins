using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace Tp.Core
{
    public interface ITaskFactory
    {
        void StartNew(Action aciton);

        Task<T> StartNew<T>(Func<T> func);
    }

    public class TpTaskFactory : ITaskFactory
    {
        private readonly bool _useThreadPool;
        private readonly TaskScheduler _taskScheduler;

        private TaskCreationOptions TaskCreationOptions => _useThreadPool ? TaskCreationOptions.None : TaskCreationOptions.LongRunning;

        public TpTaskFactory() : this(true)
        {
        }

        public TpTaskFactory(bool useThreadPool, int maxDegreeOfParallelism = default)
        {
            _useThreadPool = useThreadPool;
            _taskScheduler = maxDegreeOfParallelism == default
                ? TaskScheduler.Current
                : new LimitedConcurrencyLevelTaskScheduler(maxDegreeOfParallelism);
        }

        public void StartNew(Action action)
        {
            if (IsStartingLimitedFromLimited)
            {
                action();
            }
            else
            {
                Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions, _taskScheduler);
            }
        }

        public Task<T> StartNew<T>(Func<T> func)
        {
            return IsStartingLimitedFromLimited
                ? Task.FromResult(func())
                : Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions, _taskScheduler);
        }

        private bool IsStartingLimitedFromLimited => _taskScheduler is LimitedConcurrencyLevelTaskScheduler
            && TaskScheduler.Current == _taskScheduler;
    }
}
