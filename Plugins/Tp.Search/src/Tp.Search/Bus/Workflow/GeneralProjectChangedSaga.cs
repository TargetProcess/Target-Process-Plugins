// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Messages;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
    class GeneralProjectChangedSaga
        : TpSaga<GeneralProjectChangeSagaData>,
          IAmStartedByMessages<GeneralProjectChangedLocalMessage>,
          IHandleMessages<CommentQueryResult>,
          IHandleMessages<TestStepQueryResult>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        private readonly IActivityLogger _logger;
        private readonly CommentsIndexing _commentsIndexing;
        private readonly TestStepsIndexing _testStepsIndexing;

        public GeneralProjectChangedSaga()
        {
        }

        public GeneralProjectChangedSaga(IEntityIndexer entityIndexer, IEntityTypeProvider entityTypesProvider, IActivityLogger logger)
        {
            _logger = logger;
            _commentsIndexing = new CommentsIndexing(entityIndexer, () => Data, entityTypesProvider, d => _testStepsIndexing.Start(),
                q =>
                {
                    q.GeneralId = Data.GeneralId;
                    Send(q);
                }
                , _logger,
                (dto, indexer) =>
                        indexer.UpdateCommentIndex(dto, new List<CommentField>(), true, false, DocumentIndexOptimizeSetup.NoOptimize));

            _testStepsIndexing = new TestStepsIndexing(entityIndexer, () => Data, entityTypesProvider, d => MarkAsComplete(),
                q =>
                {
                    q.TestCaseId = Data.GeneralId;
                    Send(q);
                }
                , _logger,
                (dto, indexer) =>
                    indexer.UpdateTestStepIndex(dto, new List<TestStepField>(), Maybe.Return(Data.ProjectId),
                        DocumentIndexOptimizeSetup.NoOptimize));
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<GeneralProjectChangedLocalMessage>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<CommentQueryResult>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<TestStepQueryResult>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
        }

        public void Handle(GeneralProjectChangedLocalMessage message)
        {
            Data.ProjectId = message.ProjectId;
            Data.GeneralId = message.GeneralId;
            if (message.GeneralId > 0)
            {
                _commentsIndexing.Start();
            }
            else
            {
                MarkAsComplete();
            }
        }

        public void Handle(CommentQueryResult message)
        {
            _commentsIndexing.Handle(message);
        }

        public void Handle(TestStepQueryResult message)
        {
            _testStepsIndexing.Handle(message);
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            _logger.Error("Rebuild indexes for Comments and Test Steps failed", new Exception(message.ExceptionString));
            MarkAsComplete();
        }
    }

    public class GeneralProjectChangeSagaData : ISagaEntity, ICommentIndexingSagaData, ITestStepIndexingSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public int CommentsRetrievedCount { get; set; }
        public int CommentsCurrentDataWindowSize { get; set; }

        public int TestStepsRetrievedCount { get; set; }
        public int TestStepsCurrentDataWindowSize { get; set; }

        public int? ProjectId { get; set; }
        public int? GeneralId { get; set; }
    }
}
