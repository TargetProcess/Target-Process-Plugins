using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
    /// <summary>
    /// Base message indicating that some entity was updated in TargetProcess.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of deleted entity.</typeparam>
    /// <typeparam name="TEntityField">The array of changed fields</typeparam>
    [Serializable]
    public class EntityUpdatedMessage<TEntityDto, TEntityField> : EntityMessage<TEntityDto>, IEntityUpdatedMessage<TEntityDto>
        where TEntityDto : DataTransferObject, new()
    {
        public EntityUpdatedMessage()
        {
            ChangedFields = Array.Empty<TEntityField>();
        }

        /// <summary>
        /// The array of changes fields.
        /// </summary>
        public TEntityField[] ChangedFields { get; set; }

        /// <summary>
        /// The updated entity.
        /// </summary>
        public TEntityDto OriginalDto { get; set; }

        public IEnumerable<string> GetChangedFields()
        {
            return ChangedFields?.Select(x => x.ToString()) ?? Enumerable.Empty<string>();
        }
    }
}
