using System;
using System.Threading.Tasks;

namespace Tp.Core
{
    public interface ITaskFactory
    {
        Task StartNew(Action aciton);

        Task<T> StartNew<T>(Func<T> func);
    }

    public class TpTaskFactory : ITaskFactory
    {
        private readonly bool _useThreadPool;

        private TaskCreationOptions TaskCreationOptions => _useThreadPool ? TaskCreationOptions.None : TaskCreationOptions.LongRunning;

        public TpTaskFactory(bool useThreadPool)
        {
            _useThreadPool = useThreadPool;
        }

        public TpTaskFactory()
        {
            _useThreadPool = true;
        }

        public Task StartNew(Action action)
        {
            return Task.Factory.StartNew(action, TaskCreationOptions);
        }

        public Task<T> StartNew<T>(Func<T> func)
        {
            return Task.Factory.StartNew(func, TaskCreationOptions);
        }
    }
}
