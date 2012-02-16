// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.SourceControl.Testing.Repository.Git
{
	public class GitTestRepositoryWithFileDeleted : VcsTestRepository<GitTestRepositoryWithFileDeleted>
	{
		protected override string Name
		{
			get { return "TestRepositoryWithFileDeleted"; }
		}

		public override string Login
		{
			get { throw new System.NotImplementedException(); }
		}

		public override string Password
		{
			get { throw new System.NotImplementedException(); }
		}

		public override void Commit(string commitComment)
		{
			throw new System.NotImplementedException();
		}

		public override string Commit(string filePath, string changedContent, string commitComment)
		{
			throw new System.NotImplementedException();
		}

		public override void CheckoutBranch(string branch)
		{
			throw new System.NotImplementedException();
		}

		public override string CherryPick(string revisionId)
		{
			throw new System.NotImplementedException();
		}
	}
}