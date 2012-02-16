// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.SourceControl.Comments.Actions
{
	public class PostTimeAction : Action
	{
		public decimal TimeSpent { get; set; }
		public decimal? TimeLeft { get; set; }
		public int? UserId { get; set; }
		public int? EntityId { get; set; }
		public string Description { get; set; }

		protected override void Visit(IActionVisitor visitor)
		{
			visitor.Accept(this);
		}

		protected override bool CanBeExecuted
		{
			get { return UserId.HasValue && EntityId.HasValue; }
		}

		protected override ITargetProcessCommand CreateCommand()
		{
			return new PostTimeCommand
			{
				EntityId = EntityId,
				Description = Description,
				Spent = TimeSpent,
				UserID = UserId.GetValueOrDefault(),
				Left = TimeLeft
			};
		}

		protected override void Log(IActivityLogger logger)
		{
			logger.InfoFormat("Posting time. Entity ID: {0}; User ID: {3}; Time spent: {1:0.00}; Time left: {2:0.00}", EntityId, TimeSpent, TimeLeft, UserId);
		}
	}
}