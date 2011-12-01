// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.SourceControl.Comments.Actions;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl.Comments
{
	public static class ActionFiltersExtensions
	{
		public static IEnumerable<IAction> FillChangeStatusActionComment(this IEnumerable<IAction> actions)
		{
			var changeStatusAction = actions.OfType<ChangeStatusAction>().FirstOrDefault();
			if (changeStatusAction == null)
			{
				return actions;
			}

			var commentAction = actions.OfType<PostCommentAction>().FirstOrDefault();
			if (commentAction != null)
			{
				changeStatusAction.Comment = commentAction.Comment;
			}

			return actions.Where(x => !x.Equals(commentAction));
		}

		public static IEnumerable<AssignRevisionToEntityAction> MergeActionsWithSameEntityId(this IEnumerable<AssignRevisionToEntityAction> actions)
		{
			return actions.GroupBy(x => x).Select(x =>
			                                      	{
			                                      		var action = new AssignRevisionToEntityAction
			                                      		             	{
			                                      		             		Dto = x.Key.Dto,
			                                      		             		EntityId = x.Key.EntityId,
			                                      		             		RevisionId = x.Key.RevisionId,
			                                      		             		SagaId = x.Key.SagaId
			                                      		             	};

			                                      		action.Children.AddRange(x.SelectMany(y => y.Children.ToList()));

			                                      		return action;
			                                      	});
		}
	}
}