using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;

namespace Tp.MashupManager.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class SynchronousMashupInfoRepositoryTests
    {
        [Test]
        public void ShouldExecuteOperationsOneByOne()
        {
            var account1 = "test-account-1";
            var account2 = "test-account-2";

            var accountLocker = new AccountLocker();
            var events = new ConcurrentQueue<(string, EventType)>();

            var repo1 = CreateRepository(account1, accountLocker, events);
            var repo2 = CreateRepository(account1, accountLocker, events);
            var repo3 = CreateRepository(account2, accountLocker, events);

            var operations = new List<Task>
            {
                Task.Run(() => repo1.syncRepository.Add(new Mashup(new List<MashupFile>()), false)),
                Task.Run(() => repo2.syncRepository.Add(new Mashup(new List<MashupFile>()), false)),
            };

            Assert.That(events, Has.Count.EqualTo(1).After(150, 30));

            operations.Add(Task.Run(() => repo3.syncRepository.Delete("mashup-name")));

            Assert.That(
                events,
                Is.EquivalentTo(new[]
                {
                    (account1, EventType.Begin),
                    (account2, EventType.Begin),
                }).After(150, 30));

            repo1.innerRepository.OperationEvent.Set();
            repo2.innerRepository.OperationEvent.Set();
            repo3.innerRepository.OperationEvent.Set();

            Task.WaitAll(operations.ToArray());

            Assert.That(
                events,
                Is.EquivalentTo(new[]
                {
                    (account1, EventType.Begin),
                    (account2, EventType.Begin),
                    (account1, EventType.End),
                    (account2, EventType.End),
                    (account1, EventType.Begin),
                    (account1, EventType.End),
                }));
        }

        private (SynchronousMashupInfoRepository syncRepository, ControlledMashupInfoRepository innerRepository) CreateRepository(
            string account,
            IAccountLocker locker,
            ConcurrentQueue<(string, EventType)> events)
        {
            var pluginContext = MockRepository.GenerateStub<IPluginContext>();
            pluginContext.Stub(ctx => ctx.AccountName).Return(new AccountName(account));

            var innerRepository = new ControlledMashupInfoRepository(account, events);

            var syncRepository = new SynchronousMashupInfoRepository(
                pluginContext,
                locker,
                innerRepository);

            return (syncRepository, innerRepository);
        }
    }

    public enum EventType
    {
        Begin,
        End
    }

    public class ControlledMashupInfoRepository : IMashupInfoRepository
    {
        private readonly string _account;
        private readonly ConcurrentQueue<(string, EventType)> _events;
        public ManualResetEvent OperationEvent { get; }

        public ControlledMashupInfoRepository(string account, ConcurrentQueue<(string, EventType)> events)
        {
            _account = account;
            _events = events;
            OperationEvent = new ManualResetEvent(false);
        }

        public PluginProfileErrorCollection Add(Mashup dto, bool generateUniqueName) => PerformOperation();

        public PluginProfileErrorCollection Update(UpdateMashupCommandArg commandArg) => PerformOperation();

        public PluginProfileErrorCollection Delete(string mashupName) => PerformOperation();

        public PluginProfileErrorCollection PerformOperation()
        {
            _events.Enqueue((_account, EventType.Begin));
            OperationEvent.WaitOne();
            _events.Enqueue((_account, EventType.End));

            return new PluginProfileErrorCollection();
        }
    }
}
