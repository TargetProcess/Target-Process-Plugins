using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace Tp.Core
{
    public interface ITaskFactory
    {
        int? GetQueuedOrRunningTaskCount();
        Task StartNew(Action action);
        Task StartNew(Action action, CancellationToken cancellationToken);
        Task<T> StartNew<T>(Func<T> func);
        Task<T> StartNew<T>(Func<T> func, CancellationToken cancellationToken);
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
                ? TaskScheduler.Default
                : new LimitedConcurrencyLevelTaskScheduler(maxDegreeOfParallelism);
            IsLimitedTaskScheduler = _taskScheduler is LimitedConcurrencyLevelTaskScheduler;
        }

        public bool IsLimitedTaskScheduler { get; }

        public Task StartNew(Action action) => StartNewInternal(action, CancellationToken.None);

        public Task StartNew(Action action, CancellationToken cancellationToken) =>
            StartNewInternal(action, cancellationToken);

        public Task<T> StartNew<T>(Func<T> func) => StartNewInternal(func, CancellationToken.None);

        public Task<T> StartNew<T>(Func<T> func, CancellationToken cancellationToken) =>
            StartNewInternal(func, cancellationToken);

        private Task StartNewInternal(Action action, CancellationToken cancellationToken)
        {
            if (IsStartingLimitedFromLimited)
            {
                action();
                return Task.CompletedTask;
            }

            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions, _taskScheduler);
        }

        private Task<T> StartNewInternal<T>(Func<T> func, CancellationToken cancellationToken)
        {
            return IsStartingLimitedFromLimited
                ? Task.FromResult(func())
                : Task.Factory.StartNew(func, cancellationToken, TaskCreationOptions, _taskScheduler);
        }

        private bool IsStartingLimitedFromLimited => IsLimitedTaskScheduler && TaskScheduler.Current == _taskScheduler;

        int? ITaskFactory.GetQueuedOrRunningTaskCount()
        {
            if (_taskScheduler is LimitedConcurrencyLevelTaskScheduler limited)
            {
                return limited.GetQueuedOrRunningCount();
            }

            return null;
        }
    }
}
