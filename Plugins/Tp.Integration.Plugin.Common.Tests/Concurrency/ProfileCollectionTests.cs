// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using System.Threading;
using NUnit.Framework;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Tests.Concurrency.Utils;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Concurrency
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class ProfileCollectionTests : DomainObjectConcurrencyTest
	{
		private const string ACCOUNT_NAME = "Account";

		protected override void OnInit()
		{
			base.OnInit();
			AccountCollection.GetOrCreate(ACCOUNT_NAME);
		}

		[Test]
		public void ReadWrite()
		{
			AddAccountProfile(ACCOUNT_NAME, "profile1");
			ExecuteConcurrently(() => ReadAccountProfiles(ACCOUNT_NAME), () => AddAccountProfile(ACCOUNT_NAME, "profile2"));
			AccountCollection.GetOrCreate(ACCOUNT_NAME).Profiles.Count().Should(Be.EqualTo(2));
		}
		
		[Test]
		public void ReadRead()
		{
			AddAccountProfile(ACCOUNT_NAME, "profile1");
			ExecuteConcurrently(() => ReadAccountProfiles(ACCOUNT_NAME), () => ReadAccountProfiles(ACCOUNT_NAME));
			AccountCollection.GetOrCreate(ACCOUNT_NAME).Profiles.Count().Should(Be.EqualTo(1));
		}
		
		[Test]
		public void WriteOnceReadTwice()
		{
			int beforeProfileAdd = 0;
			int afterProfileAdded = 0;
			var account1Name = new AccountName("account1");
			AccountCollection.GetOrCreate(account1Name);
			ExecuteConcurrently(() =>
			{
				Interlocked.Increment(ref beforeProfileAdd);
				IAccount account = AccountCollection.GetOrCreate(account1Name);
				account.Profiles.Add(new ProfileCreationArgs(new ProfileName("~"), new object()));
				Interlocked.Increment(ref afterProfileAdded);
			}, () =>
			{
				var accountFirstVersion = AccountCollection.GetOrCreate(account1Name);
				var accountSecondVersion = AccountCollection.GetOrCreate(account1Name);
				bool sameVersion = false;
				if (Interlocked.Increment(ref beforeProfileAdd) == 1)
				{
					sameVersion = (accountFirstVersion == accountSecondVersion);
					sameVersion.Should(Be.True);
				}
				if (Interlocked.Increment(ref afterProfileAdded) == 2)
				{
					IAccount latestAccountVersion = AccountCollection.GetOrCreate(account1Name);
					latestAccountVersion.Profiles.Count().Should(Be.EqualTo(1));
					if (sameVersion)
					{
						(latestAccountVersion == accountFirstVersion).Should(Be.False);
					}
				}
			});
			AccountCollection.GetOrCreate(account1Name).Profiles.Count().Should(Be.EqualTo(1));
		}

		[Test, Ignore]
		public void ReadWriteChess()
		{
			Chess.RunRemotely(() => ReadWrite());
		}

		[Test, Ignore]
		public void ReadReadChess()
		{
			Chess.RunRemotely(() => ReadRead());
		}

		[Test, Ignore]
		public void WriteOnceReadTwiceChess()
		{
			Chess.RunRemotely(() => WriteOnceReadTwice());
		}

		private void AddAccountProfile(string accountName, string profileName)
		{
			AccountCollection.GetOrCreate(accountName).Profiles.Add(new ProfileCreationArgs(profileName, new object()));
		}

		private void ReadAccountProfiles(string accountName)
		{
			var account = AccountCollection.GetOrCreate(accountName);
			foreach (var profile in account.Profiles)
			{
				profile.Name.Value.Should(Be.StringContaining("profile"));
			}
		}
	}
}