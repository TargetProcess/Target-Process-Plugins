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
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Bugzilla.ImportToTp
{
	public class CommentImportSaga : TpSaga<BugCommentImportSagaData>,
	                                 IAmStartedByMessages<NewBugImportedToTargetProcessMessage>,
	                                 IAmStartedByMessages<ExistingBugImportedToTargetProcessMessage>,
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

		public void Handle(NewBugImportedToTargetProcessMessage message)
		{
			var comments = GetComments(message.BugzillaBug);

			CreateComments(comments.Cast<long_desc>(), message.TpBugId);
		}

		public void Handle(ExistingBugImportedToTargetProcessMessage message)
		{
			var newComments =
				GetComments(message.BugzillaBug).Cast<long_desc>()
					.Where(comment => !CommentExists(message, comment))
					.Reverse()
					.ToList();

			if (newComments.Any())
			{
				_logger.InfoFormat("Importing {1} comment(s) for bug. {0}", message.BugzillaBug.ToString(), newComments.Count);
			}

			CreateComments(newComments, message.TpBugId);
		}

		private bool CommentExists(ExistingBugImportedToTargetProcessMessage message, long_desc comment)
		{
			return StorageRepository().Get<CommentDTO>(message.TpBugId.ToString())
				.Where(c => c.CreateDate.Value.ToUniversalTime() == CreateDateConverter.ParseToUniversalTime(comment.bug_when))
				.Any();
		}

		private void CreateComments(IEnumerable<long_desc> comments, int? tpBugId)
		{
			foreach (var comment in comments)
			{
				CreateComment(tpBugId, comment);
			}

			CompleteSagaIfNecessary();
		}

		private long_descCollection GetComments(BugzillaBug bugzillaBug)
		{
			var comments = new long_descCollection();

			if (bugzillaBug.long_descCollection.Count >= 2)
				comments.AddRange(bugzillaBug.long_descCollection.Cast<long_desc>().Skip(1).ToList());
			
			return comments;
		}

		private void CreateComment(int? tpBugId, long_desc bugzillaComment)
		{
			if (string.IsNullOrEmpty(bugzillaComment.thetext))
				return;

			var userId = ObjectFactory.GetInstance<IUserMapper>().GetTpIdBy(bugzillaComment.who);

			var comment = new CommentDTO
			              	{
			              		Description = DescriptionConverter.FormatDescription(bugzillaComment.thetext),
			              		GeneralID = tpBugId,
			              		CreateDate = CreateDateConverter.ParseFromBugzillaLocalTime(bugzillaComment.bug_when),
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

	public class BugCommentImportSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int CommentsToCreateCount { get; set; }
		public int CreatedCommentsCount { get; set; }
	}
}