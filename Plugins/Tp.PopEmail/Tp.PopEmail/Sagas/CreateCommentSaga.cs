// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;

namespace Tp.PopEmailIntegration.Sagas
{
    public class CreateCommentSaga
        : TpSaga<CreateCommentSagaData>, IAmStartedByMessages<CreateCommentCommandInternal>,
          IHandleMessages<CommentCreatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<CommentCreatedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(CreateCommentCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            Data.Comment = message.Comment;
            Send(new CreateCommentCommand(message.Comment));
        }

        public void Handle(CommentCreatedMessage message)
        {
            SendLocal(new CommentCreatedMessageInternal { SagaId = Data.OuterSagaId, Comment = Data.Comment });
            MarkAsComplete();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            Log().Error($"Failed to create comment '{Data.Comment.Description}'",
                message.GetException());
            SendLocal(new CommentCreateFailedMessageInternal { SagaId = Data.OuterSagaId });
            MarkAsComplete();
        }
    }

    [Serializable]
    public class CreateCommentCommandInternal : IPluginLocalMessage
    {
        public Guid OuterSagaId { get; set; }
        public CommentDTO Comment { get; set; }
    }

    [Serializable]
    public class CommentCreatedMessageInternal : SagaMessage, IPluginLocalMessage
    {
        public CommentDTO Comment { get; set; }
    }

    [Serializable]
    public class CommentCreateFailedMessageInternal : SagaMessage, IPluginLocalMessage
    {
    }

    [Serializable]
    public sealed class MigrateUsersCommandInternal : SagaMessage, IPluginLocalMessage
    {
    }

    [Serializable]
    public class CreateCommentSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public CommentDTO Comment { get; set; }
        public Guid OuterSagaId { get; set; }
    }
}
