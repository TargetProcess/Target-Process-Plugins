// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.SourceControl.Comments.Actions
{
	public class ChangeStatusAction : Action
	{
		public string Status { get; set; }

		public string DefaultComment
		{
			get
			{
				return String.Format("State is changed by '{0}' plugin",
				                     ObjectFactory.GetInstance<IPluginMetadata>().PluginData.Name);
			}
		}

		public string Comment { get; set; }

		public int? EntityId { get; set; }

		public int? UserId { get; set; }

		protected override bool CanBeExecuted
		{
			get { return EntityId.HasValue && UserId.HasValue; }
		}

		protected override ITargetProcessCommand CreateCommand()
		{
			return new ChangeEntityStateCommand
			       	{
			       		EntityId = EntityId,
			       		State = Status,
			       		UserID = UserId.GetValueOrDefault(),
			       		Comment = Comment,
			       		DefaultComment = DefaultComment
			       	};
		}

		protected override void Log(IActivityLogger logger)
		{
			logger.InfoFormat("Changing entity state. Entity ID: {0}; Entity State: {1}; Comment: {2}; Default comment: {3}", EntityId, Status, Comment, DefaultComment);
		}

		protected override void Visit(IActionVisitor visitor)
		{
			visitor.Accept(this);
		}
	}
}