using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Synchronizer;
using Tp.Testing.Common.NUnit;
using System.Linq;

namespace Tp.MashupManager.Tests.MashupLibrary.Repository.Synchronizer
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class LibraryRepositorySynchronizerTests
    {
        [SetUp]
        public void SetUp()
        {
            _repository = MockRepository.GenerateStub<ISynchronizableLibraryRepository>();
            _repository.Stub(x => x.Id).Return("TestRepository");
            _synchronizer = new LibraryRepositorySynchronizer();
        }

        private ISynchronizableLibraryRepository _repository;
        private LibraryRepositorySynchronizer _synchronizer;
        private int _currentReadersCount;
        private int _currentWritersCount;
        private int _unexpectedSymultaneousThreadTotal;

        private Task CreateReadTask()
        {
            var task = new Task(() =>
            {
                _synchronizer.BeginRead(_repository);
                try
                {
                    Interlocked.Increment(ref _currentReadersCount);
                    Thread.Sleep(1);
                }
                finally
                {
                    Interlocked.Decrement(ref _currentReadersCount);
                    Interlocked.Add(ref _unexpectedSymultaneousThreadTotal, _currentWritersCount);
                    _synchronizer.EndRead(_repository);
                }
            });
            return task;
        }

        private Task CreateWriteTask()
        {
            var task = new Task(() =>
            {
                if (_synchronizer.TryBeginWrite(_repository))
                {
                    try
                    {
                        Interlocked.Increment(ref _currentWritersCount);
                        Thread.Sleep(1);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _currentWritersCount);
                        Interlocked.Add(ref _unexpectedSymultaneousThreadTotal, _currentReadersCount);
                        Interlocked.Add(ref _unexpectedSymultaneousThreadTotal, _currentWritersCount);
                        _synchronizer.EndWrite(_repository);
                    }
                }
            });

            return task;
        }


        [Test]
        public void RepositoryReadWriteShouldBeSynchronized()
        {
            var tasks = new Task[10000];
            for (var i = 0; i < 10000; i++)
            {
                tasks[i] = (i % 2) != 0 ? CreateReadTask() : CreateWriteTask();
            }
            tasks.ForEach(x => x.Start());
            Task.WaitAll(tasks);

            _unexpectedSymultaneousThreadTotal.Should(Be.EqualTo(0), "_unexpectedSymultaneousThreadTotal.Should(Be.EqualTo(0))");
        }
    }
}
