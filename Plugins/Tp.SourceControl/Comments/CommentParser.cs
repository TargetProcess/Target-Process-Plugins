// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using TinyPG;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Comments.DSL;
using Tp.SourceControl.Messages;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Comments
{
	public class CommentParser
	{
		private readonly Parser _parser = new Parser(new Scanner());
		private readonly IActivityLogger _log = ObjectFactory.GetInstance<IActivityLogger>();

		public IEnumerable<AssignRevisionToEntityAction> ParseAssignToEntityAction(RevisionDTO revision)
		{
			return ParseAssignToEntityAction(revision.Description);
		}
		public IEnumerable<AssignRevisionToEntityAction> ParseAssignToEntityAction(RevisionInfo revision)
		{
			return ParseAssignToEntityAction(revision.Comment);
		}

		private IEnumerable<AssignRevisionToEntityAction> ParseAssignToEntityAction(string revisionComment)
		{
			return GetActions(revisionComment);
		}

		public IEnumerable<IAction> Parse(RevisionDTO revision, int entityId)
		{
			return GetActions(revision.Description)
				.Where(x => x.EntityId == entityId)
				.SelectMany(x => x.Children.FillChangeStatusActionComment());
		}

		private IEnumerable<AssignRevisionToEntityAction> GetActions(string description)
		{
			if (string.IsNullOrEmpty(description))
			{
				return Enumerable.Empty<AssignRevisionToEntityAction>();
			}

			try
			{
				var commandTree = _parser.Parse(description.Trim() + " ");

				if (commandTree.Errors.Count > 0)
				{
					_log.Error(new CommentFailedToParseException(commandTree.Errors, description));
				}

				var actions = (List<AssignRevisionToEntityAction>)commandTree.Eval();

				return actions.MergeActionsWithSameEntityId();
			}
			catch(Exception exception)
			{
				_log.Error(string.Format("Failed to parse comment {0}", description), exception);
				return Enumerable.Empty<AssignRevisionToEntityAction>();
			}
		}
	}
}