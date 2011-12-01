// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NUnit.Framework;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Integration.Plugin.Common.Tests.Concurrency.Utils;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Concurrency
{
	[TestFixture]
	public class ProfileTests : DomainObjectConcurrencyTest
	{
		private IAccount _account;
		private const string ACCOUNT_NAME = "Account";

		protected override void OnInit()
		{
			base.OnInit();
			_account = AccountCollection.GetOrCreate(ACCOUNT_NAME);
		}

		[Test]
		public void ReadWrite()
		{
			_account.Profiles.Add(new ProfileCreationArgs("Profile", new SampleJiraProfile {JiraLogin = "JiraLogin"}));
			ExecuteConcurrently(UpdateProfile, ReadProfile);
			AccountCollection.GetOrCreate(ACCOUNT_NAME).Profiles.Count().Should(Be.EqualTo(1));
		}
		
		[Test]
		public void ReadRead()
		{
			_account.Profiles.Add(new ProfileCreationArgs("Profile", new SampleJiraProfile { JiraLogin = "JiraLogin" }));
			ExecuteConcurrently(ReadProfile, ReadProfile);
			AccountCollection.GetOrCreate(ACCOUNT_NAME).Profiles.Count().Should(Be.EqualTo(1));
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

		private void UpdateProfile()
		{
			var profile = _account.Profiles["Profile"];
			profile.Settings = new SampleJiraProfile {JiraLogin = "JiraLogin"};
			profile.Save();
		}

		private void ReadProfile()
		{
			var profile = _account.Profiles["Profile"];
			profile.GetProfile<SampleJiraProfile>().JiraLogin.Should(Be.StringContaining("JiraLogin"));
		}
	}
}