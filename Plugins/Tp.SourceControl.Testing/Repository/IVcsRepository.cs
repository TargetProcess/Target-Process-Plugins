// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.SourceControl.Testing.Repository
{
	public interface IVcsRepository : IVcsCredentials
	{
		void Commit(string commitComment);

		string Commit(string filePath, string changedContent, string commitComment);

		void CheckoutBranch(string branch);

		string CherryPick(string revisionId);
	}
}