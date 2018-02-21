using System;
using System.Collections.Generic;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
    public interface IEntityUpdatedMessage<out TEntityDto> : ITargetProcessMessage where TEntityDto : IDataTransferObject
    {
        /// <summary>
        /// The array of changes fields.
        /// </summary>
        IEnumerable<string> GetChangedFields();

        /// <summary>
        /// The updated entity.
        /// </summary>
        TEntityDto Dto { get; }

        /// <summary>
        /// The updated entity.
        /// </summary>
        TEntityDto OriginalDto { get; }

        /// <summary>
        /// The author of changes
        /// </summary>
        GeneralUserDTO Author { get; }

        /// <summary>
        /// The date of message creating
        /// </summary>
        /// 
        DateTime? CreateDate { get; }
    }
}
