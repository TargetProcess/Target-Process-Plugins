// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Web;
using Tp.SourceControl.Comments.Actions;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl.Comments.DSL
{
	public class ActionFactory : IActionFactory
	{
		public IAction CreateAssignRevisionToEntityAction(int entityId)
		{
			return new AssignRevisionToEntityAction {EntityId = entityId};
		}

		public IAction CreatePostTimeAction(decimal timeSpent, decimal? timeLeft)
		{
			return new PostTimeAction {TimeSpent = timeSpent, TimeLeft = timeLeft};
		}

		public IAction CreatePostCommentAction(string comment)
		{
			return new PostCommentAction {Comment = HttpUtility.HtmlEncode(comment)};
		}

		public IAction CreateChangeStatusAction(string status)
		{
			return new ChangeStatusAction {Status = status};
		}
	}
}