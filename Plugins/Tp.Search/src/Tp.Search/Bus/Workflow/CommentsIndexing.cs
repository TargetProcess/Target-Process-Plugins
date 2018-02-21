using System;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
    public interface ICommentIndexingSagaData
    {
        int CommentsRetrievedCount { get; set; }
        int CommentsCurrentDataWindowSize { get; set; }
    }

    class CommentsIndexing : IndexAlgorithm<CommentDTO, ICommentIndexingSagaData, CommentQuery>
    {
        private readonly Action<CommentDTO, IEntityIndexer> _indexMethod;

        public CommentsIndexing(IEntityIndexer entityIndexer, Func<ICommentIndexingSagaData> data, IEntityTypeProvider entityTypesProvider,
            Action<ICommentIndexingSagaData> onComplete, Action<CommentQuery> sendQuery, IActivityLogger logger,
            Action<CommentDTO, IEntityIndexer> indexMethod)
            : base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "comment")
        {
            _indexMethod = indexMethod;
        }

        protected override void IndexEntity(CommentDTO dto)
        {
            _indexMethod(dto, EntityIndexer);
        }

        protected override void OptimizeIndex()
        {
            EntityIndexer.OptimizeCommentIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
        }

        protected override void IncrementCounters(int count)
        {
            Data.CommentsRetrievedCount += count;
            Data.CommentsCurrentDataWindowSize += count;
        }

        protected override int GetCurrentDataWindowSize()
        {
            return Data.CommentsCurrentDataWindowSize;
        }

        protected override void ResetCurrentDataWindowSize()
        {
            Data.CommentsCurrentDataWindowSize = 0;
        }

        protected override int GetTotalRetrievedEntitiesCount()
        {
            return Data.CommentsRetrievedCount;
        }

        protected override void ResetTotalRetrievedEntitiesCount()
        {
            Data.CommentsRetrievedCount = 0;
        }

        protected override CommentQuery CreateQuery()
        {
            return new CommentQuery
            {
                GeneralEntityTypes = EntityTypesProvider.EntityTypeIds.ToArray()
            };
        }
    }
}
