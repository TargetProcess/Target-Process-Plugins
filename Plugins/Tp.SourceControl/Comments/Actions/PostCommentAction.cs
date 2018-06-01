// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.SourceControl.Comments.Actions
{
    public class PostCommentAction : Action
    {
        public int? UserId { get; set; }

        public int? EntityId { get; set; }

        public string Comment { get; set; }

        protected override bool CanBeExecuted => EntityId.HasValue && UserId.HasValue;

        protected override ITargetProcessCommand CreateCommand()
        {
            var dto = new CommentDTO { GeneralID = EntityId, Description = Comment, OwnerID = UserId };
            return new CreateCommand { Dto = dto };
        }

        protected override void Log(IActivityLogger logger)
        {
            logger.Info($"Posting comment. Entity ID: {EntityId}; Comment: {Comment}; Author: {UserId}");
        }

        protected override void Visit(IActionVisitor visitor)
        {
            visitor.Accept(this);
        }
    }
}
