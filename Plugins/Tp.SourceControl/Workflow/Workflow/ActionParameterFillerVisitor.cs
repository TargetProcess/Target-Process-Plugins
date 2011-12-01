// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.SourceControl.Comments.Actions;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class ActionParameterFillerVisitor : IActionVisitor
	{
		private readonly RevisionDTO _dto;
		private readonly int? _entityId;

		public ActionParameterFillerVisitor(RevisionDTO dto, int? entityId = null)
		{
			_dto = dto;
			_entityId = entityId;
		}

		public void Accept(PostTimeAction action)
		{
			action.EntityId = _entityId;
			action.UserId = _dto.AuthorID;
			action.Description = _dto.Description;
		}

		public void Accept(AssignRevisionToEntityAction action)
		{
			action.RevisionId = _dto.RevisionID;
			action.Dto = _dto;
		}

		public void Accept(PostCommentAction action)
		{
			action.EntityId = _entityId;
			action.UserId = _dto.AuthorID;
		}

		public void Accept(ChangeStatusAction action)
		{
			action.EntityId = _entityId;
			action.UserId = _dto.AuthorID;
		}
	}
}