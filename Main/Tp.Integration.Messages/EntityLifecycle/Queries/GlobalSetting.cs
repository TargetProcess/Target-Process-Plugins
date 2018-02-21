using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class GlobalSettingQueryResult : QueryResult<GlobalSettingDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class RetrieveGlobalSettingQuery : QueryBase
    {
        public override DtoType DtoType
        {
            get { return new DtoType(typeof(GlobalSettingDTO)); }
        }
    }
}
