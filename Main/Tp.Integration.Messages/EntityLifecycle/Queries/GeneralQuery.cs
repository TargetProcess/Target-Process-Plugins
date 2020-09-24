using System;
using System.Runtime.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class GeneralQuery : QueryBase
    {
        [OptionalField] private bool? _isExtendableDomainOnly = false;

        public int?[] EntityTypes { get; set; }
        // TODO remove it after ED migration performed (IndexExtendableDomainEntitiesMigration) US#260236
        public bool? IsExtendableDomainOnly
        {
            get => _isExtendableDomainOnly;
            set => _isExtendableDomainOnly = value;
        }

        public override DtoType DtoType
        {
            get { return new DtoType(typeof(GeneralDTO)); }
        }
    }

    [Serializable]
    public class GeneralQueryResult : QueryResult<GeneralDTO>, ISagaMessage
    {
    }
}
