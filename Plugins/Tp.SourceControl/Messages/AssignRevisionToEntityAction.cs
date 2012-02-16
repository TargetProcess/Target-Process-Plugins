// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Comments;

namespace Tp.SourceControl.Messages
{
	public class AssignRevisionToEntityAction : SagaMessage, IAction
	{
		public AssignRevisionToEntityAction()
		{
			Children = new List<IAction>();
		}

		public int EntityId { get; set; }

		public int? RevisionId { get; set; }

		public RevisionDTO Dto { get; set; }

		public List<IAction> Children { get; private set; }

		public ITargetProcessCommand CreateCommand()
		{
			var dto = new RevisionAssignableDTO
			          	{
			          		AssignableID = EntityId,
			          		RevisionID = RevisionId
			          	};

			return new CreateCommand {Dto = dto};
		}

		public void Execute(IActionVisitor visitor, Action<ITargetProcessCommand> executor, IActivityLogger logger)
		{
			visitor.Accept(this);
			executor(CreateCommand());
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as AssignRevisionToEntityAction);
		}

		public bool Equals(AssignRevisionToEntityAction other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return other.EntityId == EntityId && other.RevisionId.Equals(RevisionId);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EntityId*397) ^ (RevisionId.HasValue ? RevisionId.Value : 0);
			}
		}
	}
}