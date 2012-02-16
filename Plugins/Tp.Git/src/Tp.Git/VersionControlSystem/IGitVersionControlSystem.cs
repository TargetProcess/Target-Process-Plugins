// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NGit.Revwalk;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	public interface IGitVersionControlSystem : IVersionControlSystem
	{
		RevCommit GetCommit(RevisionId id);
	}
}