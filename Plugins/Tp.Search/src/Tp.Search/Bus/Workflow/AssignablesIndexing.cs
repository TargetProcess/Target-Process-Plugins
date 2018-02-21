using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
    public interface IAssignableIndexingSagaData
    {
        int AssignablesRetrievedCount { get; set; }
        int AssignablesCurrentDataWindowSize { get; set; }
    }

    class AssignablesIndexing : IndexAlgorithm<AssignableDTO, IAssignableIndexingSagaData, AssignableQuery>
    {
        public AssignablesIndexing(IEntityIndexer entityIndexer, Func<IAssignableIndexingSagaData> data,
            IEntityTypeProvider entityTypesProvider, Action<IAssignableIndexingSagaData> onComplete, Action<AssignableQuery> sendQuery,
            IActivityLogger logger)
            : base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "assignable")
        {
        }

        protected override void IndexEntity(AssignableDTO dto)
        {
            EntityIndexer.UpdateAssignableIndex(dto, new[] { AssignableField.EntityStateID, AssignableField.SquadID }, true,
                DocumentIndexOptimizeSetup.NoOptimize);
        }

        protected override void OptimizeIndex()
        {
            EntityIndexer.OptimizeAssignableIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
        }

        protected override void IncrementCounters(int count)
        {
            Data.AssignablesRetrievedCount += count;
            Data.AssignablesCurrentDataWindowSize += count;
        }

        protected override int GetCurrentDataWindowSize()
        {
            return Data.AssignablesCurrentDataWindowSize;
        }

        protected override void ResetCurrentDataWindowSize()
        {
            Data.AssignablesCurrentDataWindowSize = 0;
        }

        protected override int GetTotalRetrievedEntitiesCount()
        {
            return Data.AssignablesRetrievedCount;
        }

        protected override void ResetTotalRetrievedEntitiesCount()
        {
            Data.AssignablesRetrievedCount = 0;
        }

        protected override AssignableQuery CreateQuery()
        {
            return new AssignableQuery();
        }
    }
}
