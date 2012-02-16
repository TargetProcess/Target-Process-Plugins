// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.SourceControl.Testing.Repository.Git
{
	public class GitTestRepositoryWithCherryPickedCommit : VcsTestRepository<GitTestRepositoryWithFileDeleted>
	{
		protected override string Name
		{
			get { return "TestRepositoryWithCherryPickedCommit"; }
		}

		public override string Login
		{
			get { return string.Empty; }
		}

		public override string Password
		{
			get { return string.Empty; }
		}

		public override void Commit(string commitComment)
		{
			throw new NotImplementedException();
		}

		public override string Commit(string filePath, string changedContent, string commitComment)
		{
			throw new NotImplementedException();
		}

		public override void CheckoutBranch(string branch)
		{
			throw new NotImplementedException();
		}

		public override string CherryPick(string revisionId)
		{
			throw new NotImplementedException();
		}
	}
}