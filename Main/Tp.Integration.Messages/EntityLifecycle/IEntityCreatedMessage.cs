using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
    public interface IEntityCreatedMessage<out TEntityDto> : ITargetProcessMessage where TEntityDto : IDataTransferObject
    {
        /// <summary>
        /// The updated entity.
        /// </summary>
        TEntityDto Dto { get; }

        /// <summary>
        /// The author of changes
        /// </summary>
        GeneralUserDTO Author { get; }

        /// <summary>
        /// the date of message creating
        /// </summary>
        DateTime? CreateDate { get; }
    }
}
