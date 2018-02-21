using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Document;
using Tp.Search.Model.Query;
using Tp.Search.StructureMap;
using Tp.Search.Tests.Registry;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
    using CommonPluginRegistry = Tp.Integration.Plugin.Common.StructureMap.PluginRegistry;

    public class SearchTestBase
    {
        [SetUp]
        public void SetUp()
        {
            ObjectFactory.Initialize(e =>
            {
                e.AddRegistry(new PluginRegistry());
                e.AddRegistry(new CommonPluginRegistry());
                e.AddRegistry(new TestRegistry());
            });
            OnSetup();
        }

        protected virtual void OnSetup()
        {
            ClearIndexes();
        }

        protected virtual void OnTearDown()
        {
            ClearIndexes();
        }

        [TearDown]
        public void TearDown()
        {
            OnTearDown();
        }

        private void ClearIndexes()
        {
            var logger = GetInstance<IActivityLogger>();
            GetInstance<IDocumentIndexProvider>()
                .ShutdownDocumentIndexes(new PluginContextSnapshot(GetInstance<IPluginContext>()),
                    new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true), logger);
        }

        protected T GetInstance<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }

        protected static void CheckByState(string queryString, int stateId, int[] projectIds, int resultCount = 1)
        {
            var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
            var result = queryRunner.Run(new QueryData
            {
                Query = queryString,
                EntityStateIds = new[] { stateId },
                ProjectIds = projectIds
            });
            result.Total.Should(Be.EqualTo(resultCount), "result.Total.Should(Be.EqualTo(resultCount))");
        }

        protected static ISagaMessage[] ReplyOnEntityQuery<TQuery, TDto, TQueryResult>(TQuery x, TDto[] generalDtos)
            where TQueryResult : QueryResult<TDto>, ISagaMessage, new()
            where TDto : DataTransferObject
            where TQuery : QueryBase
        {
            var generalQueryResults = generalDtos.Select(dto => new TQueryResult
            {
                Dtos = new[] { dto },
                QueryResultCount = generalDtos.Count(),
                TotalQueryResultCount = generalDtos.Count()
            }).Skip(x.Skip.GetValueOrDefault()).Take(x.Take.HasValue ? x.Take.Value : int.MaxValue);
            if (generalQueryResults.Empty())
                generalQueryResults =
                    generalQueryResults.Concat(new TQueryResult
                    {
                        Dtos = new TDto[] { },
                        QueryResultCount = 0,
                        TotalQueryResultCount = 0
                    });

            return generalQueryResults.ToArray();
        }

        protected static ISagaMessage[] ReplyOnAssignableQuery(AssignableQuery x, AssignableDTO[] assignableDtos)
        {
            var assignableQueryResults = assignableDtos.Where(a => x.ProjectId == null || a.ProjectID == x.ProjectId)
                .Select(dto => new AssignableQueryResult
                {
                    Dtos = new[] { dto },
                    QueryResultCount = assignableDtos.Count(),
                    TotalQueryResultCount = assignableDtos.Count()
                })
                .Skip(x.Skip.GetValueOrDefault())
                .Take(x.Take.HasValue ? x.Take.Value : int.MaxValue);

            if (assignableQueryResults.Empty())
                assignableQueryResults =
                    assignableQueryResults.Concat(new AssignableQueryResult
                    {
                        Dtos = new AssignableDTO[] { },
                        QueryResultCount = 0,
                        TotalQueryResultCount = 0
                    });
            return assignableQueryResults.ToArray();
        }
    }
}
