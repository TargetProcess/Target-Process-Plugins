// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.BugTracking;
using Tp.BugTracking.ImportToTp;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Bugzilla.ImportToTp
{
    public class CommentImportSaga
        : TpSaga<BugCommentImportSagaData>,
          IAmStartedByMessages<NewBugImportedToTargetProcessMessage<BugzillaBug>>,
          IAmStartedByMessages<ExistingBugImportedToTargetProcessMessage<BugzillaBug>>,
          IHandleMessages<CommentCreatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        private readonly IActivityLogger _logger;

        public CommentImportSaga()
        {
        }

        public CommentImportSaga(IActivityLogger logger)
        {
            _logger = logger;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<CommentCreatedMessage>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
        }

        public void Handle(NewBugImportedToTargetProcessMessage<BugzillaBug> message)
        {
            CreateComments(message.ThirdPartyBug.comments, message.TpBugId);
        }

        public void Handle(ExistingBugImportedToTargetProcessMessage<BugzillaBug> message)
        {
            var newComments =
                message.ThirdPartyBug.comments
                    .Where(comment => !CommentExists(message, comment))
                    .Reverse()
                    .ToList();

            if (newComments.Any())
            {
                _logger.InfoFormat("Importing {1} comment(s) for bug. {0}", message.ThirdPartyBug.ToString(), newComments.Count);
            }

            CreateComments(newComments, message.TpBugId);
        }

        private bool CommentExists(ExistingBugImportedToTargetProcessMessage<BugzillaBug> message, BugzillaComment comment)
        {
            return StorageRepository().Get<CommentDTO>(message.TpBugId.ToString())
                .Any(c => c.CreateDate.Value.ToUniversalTime() == CreateDateConverter.ParseToUniversalTime(comment.DateAdded));
        }

        private void CreateComments(IEnumerable<BugzillaComment> comments, int? tpBugId)
        {
            foreach (var comment in comments)
            {
                CreateComment(tpBugId, comment);
            }

            CompleteSagaIfNecessary();
        }

        private void CreateComment(int? tpBugId, BugzillaComment bugzillaComment)
        {
            if (string.IsNullOrEmpty(bugzillaComment.Body))
                return;

            var userId = ObjectFactory.GetInstance<IUserMapper>().GetTpIdBy(bugzillaComment.Author);

            var comment = new CommentDTO
            {
                Description = DescriptionConverter.FormatDescription(bugzillaComment.Body),
                GeneralID = tpBugId,
                CreateDate = CreateDateConverter.ParseFromBugzillaLocalTime(bugzillaComment.DateAdded),
                OwnerID = userId
            };

            Send(new CreateCommentCommand(comment));
            Data.CommentsToCreateCount++;
        }

        public void Handle(CommentCreatedMessage message)
        {
            Data.CreatedCommentsCount++;
            StorageRepository().Get<CommentDTO>(message.Dto.GeneralID.ToString()).Add(message.Dto);
            DoNotContinueDispatchingCurrentMessageToHandlers();

            CompleteSagaIfNecessary();
        }

        private void CompleteSagaIfNecessary()
        {
            if (Data.CreatedCommentsCount == Data.CommentsToCreateCount)
                MarkAsComplete();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            _logger.Error("Comment import failed", new Exception(message.ExceptionString));
            MarkAsComplete();
        }
    }
}
