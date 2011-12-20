// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Text;
using NGit;
using StructureMap;

namespace Tp.SourceControl.Testing.Repository.Git
{
	public class GitTestRepository : VcsTestRepository<GitTestRepository>
	{
		public GitTestRepository()
		{
			ObjectFactory.Configure(x => x.For<GitTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));
		}

		private NGit.Api.Git _git;

		private string ClonedRepoFolder
		{
			get { return LocalRepositoryPath + "Cloned"; }
		}

		protected override void OnTestRepositoryDeployed()
		{
			base.OnTestRepositoryDeployed();

			_git = NGit.Api.Git.CloneRepository()
				.SetURI(LocalRepositoryPath)
				.SetDirectory(ClonedRepoFolder).Call();

			BatchingProgressMonitor.ShutdownNow();
		}


		protected override string Name
		{
			get { return "TestRepository"; }
		}

		public override string Login
		{
			get { return "test"; }
		}

		public override string Password
		{
			get { return "test"; }
		}

		public override void Commit(string commitComment)
		{
			using (var file = File.OpenWrite(Path.Combine(ClonedRepoFolder, "secondFile.txt")))
			{
				var changes = new UTF8Encoding(true).GetBytes("my changed content");
				file.Write(changes, 0, changes.Length);
			}

			_git.Commit().SetMessage(commitComment).SetAuthor(Login, "admin@admin.com").Call();
			_git.Push().Call();

			BatchingProgressMonitor.ShutdownNow();
		}
	}
}