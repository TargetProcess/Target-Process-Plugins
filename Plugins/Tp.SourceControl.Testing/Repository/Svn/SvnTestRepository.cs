// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Net;
using System.Text;
using SharpSvn;
using SharpSvn.Security;
using StructureMap;
using Tp.Testing.Core;

namespace Tp.SourceControl.Testing.Repository.Svn
{
	public class SvnTestRepository : VcsTestRepository<SvnTestRepository>
	{
		public SvnTestRepository()
		{
			ObjectFactory.Configure(x => x.For<SvnTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));
		}

		protected override string Name
		{
			get { return "TestRepository"; }
		}

		protected override void OnTestRepositoryDeployed()
		{
			_client = CreateSvnClient();
			Checkout();
		}

		private void Checkout()
		{
			LocalRepositoryCheckedOutPath.DeleteDirectory();

			_client.CheckOut(new SvnUriTarget(LocalRepositoryPath), LocalRepositoryCheckedOutPath);
			_client.Update(LocalRepositoryCheckedOutPath);
		}

		private SvnClient CreateSvnClient()
		{
			var client = new SvnClient();

			client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);
			client.Authentication.SslServerTrustHandlers +=
				delegate(object sender, SvnSslServerTrustEventArgs args)
				{
					// If needed we can look at the rest of the arguments of 'args' whether 
					// we wish to accept. If accept:
					args.AcceptedFailures = args.Failures;
					args.Save = true; // Save acceptance to authentication store
				};

			client.Authentication.Clear();
			client.Authentication.UserNameHandlers +=
				delegate(object sender, SvnUserNameEventArgs e) { e.UserName = Login; };
			return client;
		}


		private SvnClient _client;

		public string Login
		{
			get { return "test"; }
		}

		public string Password
		{
			get { return "123456"; }
		}

		protected string LocalRepositoryCheckedOutPath
		{
			get { return LocalRepositoryPath + "CheckedOut"; }
		}

		public void Commit(string commitComment)
		{
			string changedFilePath = Path.Combine(LocalRepositoryCheckedOutPath, "readme.txt");
			using (var changedFile = File.OpenWrite(changedFilePath))
			{
				var changes = new UTF8Encoding(true).GetBytes("my changed content");
				changedFile.Write(changes, 0, changes.Length);
			}

			var commit = new SvnCommitArgs {LogMessage = commitComment};
			_client.Commit(LocalRepositoryCheckedOutPath, commit);
		}
	}
}