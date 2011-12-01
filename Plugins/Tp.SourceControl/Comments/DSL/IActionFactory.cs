// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.SourceControl.Comments.DSL
{
	public interface IActionFactory
	{
		IAction CreateAssignRevisionToEntityAction(int entityId);
		IAction CreatePostTimeAction(decimal timeSpent, decimal? timeLeft);
		IAction CreatePostCommentAction(string comment);
		IAction CreateChangeStatusAction(string status);
	}
}