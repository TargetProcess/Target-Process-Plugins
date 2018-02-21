using System;
using System.Threading;
using StructureMap;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Concurrency
{
    public abstract class DomainObjectConcurrencyTest : SqlPersisterSpecBase
    {
        protected IAccountCollection AccountCollection { get; private set; }

        protected override void OnInit()
        {
            base.OnInit();
            AccountCollection = ObjectFactory.GetInstance<IAccountCollection>();
        }

        protected void ExecuteConcurrently(Action firstAction, Action secondAction)
        {
            Exception threadException = null;
            var thread = new Thread(() =>
            {
                try
                {
                    firstAction();
                }
                catch (Exception e)
                {
                    threadException = e;
                }
            });
            thread.Start();
            secondAction();
            thread.Join();
            threadException.Should(Be.Null, "threadException.Should(Be.Null)");
        }
    }
}
