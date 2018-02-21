//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Linq;
using System.Threading;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Events;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Tests.Concurrency.Utils;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Concurrency
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class AccountCollectionTests : DomainObjectConcurrencyTest
    {
        [Test]
        public void WriteWrite()
        {
            ExecuteConcurrently(() => AddAccount("account1"), () => AddAccount("account2"));
            AccountCollection.Count().Should(Be.EqualTo(2), "AccountCollection.Count().Should(Be.EqualTo(2))");
        }

        [Test]
        public void ReadRead()
        {
            AddAccount("account1");
            AddAccount("account2");
            ExecuteConcurrently(ReadAccounts, ReadAccounts);
            AccountCollection.Count().Should(Be.EqualTo(2), "AccountCollection.Count().Should(Be.EqualTo(2))");
        }

        [Test]
        public void ReadWrite()
        {
            AddAccount("account1");
            ExecuteConcurrently(ReadAccounts, () => AddAccount("account2"));
            AccountCollection.Count().Should(Be.EqualTo(2), "AccountCollection.Count().Should(Be.EqualTo(2))");
        }

        [Test]
        public void Write3Remove1()
        {
            AddAccount("account1");
            ExecuteConcurrently(ReadAccounts, () => AddAccount("account2"));
            ExecuteConcurrently(() => AddAccount("account3"), () => RemoveAccount("account2"));
            AccountCollection.Count().Should(Be.EqualTo(2), "AccountCollection.Count().Should(Be.EqualTo(2))");
        }

        [Test]
        public void RemoveAndUpdateAccountTest()
        {
            ExecuteConcurrently(() => RemoveAccount("account2"), () =>
            {
                ObjectFactory.GetInstance<IEventAggregator>()
                    .Get<Event<ProfileChanged>>()
                    .Raise(new ProfileChanged(new ProfileSafeNull(), new AccountName("account2")));
                AccountCollection.GetOrCreate(new AccountName("account2"));
            });
        }

        [Test]
        public void WriteTwiceReadTwice()
        {
            int afterAccountAdded = 0;
            var account1Name = new AccountName("account1");
            var account2Name = new AccountName("account2");
            ExecuteConcurrently(() =>
            {
                AccountCollection.GetOrCreate(account1Name);
                AccountCollection.GetOrCreate(account2Name);
                Interlocked.Increment(ref afterAccountAdded);
            }, () =>
            {
                if (Interlocked.Increment(ref afterAccountAdded) == 2)
                {
                    AccountCollection.Count().Should(Be.EqualTo(2), "AccountCollection.Count().Should(Be.EqualTo(2))");
                }
            });
            AccountCollection.Count().Should(Be.EqualTo(2), "AccountCollection.Count().Should(Be.EqualTo(2))");
        }

        [Test, Ignore]
        public void ReadReadChess()
        {
            Chess.RunRemotely(() => ReadRead());
        }

        [Test, Ignore]
        public void WriteWriteChess()
        {
            Chess.RunRemotely(() => WriteWrite());
        }

        [Test, Ignore]
        public void ReadWriteChess()
        {
            Chess.RunRemotely(() => ReadWrite());
        }

        [Test, Ignore]
        public void WriteTwiceReadTwiceChess()
        {
            Chess.RunRemotely(() => WriteTwiceReadTwice());
        }

        public void ReadAccounts()
        {
            foreach (var account in AccountCollection)
            {
                account.Name.Value.Should(Be.Not.Empty, "account.Name.Value.Should(Be.Not.Empty)");
            }
        }

        public void AddAccount(string accountName)
        {
            AccountCollection.GetOrCreate(new AccountName(accountName));
        }

        public void RemoveAccount(string accountName)
        {
            AccountCollection.Remove(new AccountName(accountName));
        }
    }
}
