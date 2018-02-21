using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
    public interface IEntityDeletedMessage<out TEntityDto> : ITargetProcessMessage where TEntityDto : IDataTransferObject
    {
        TEntityDto Dto { get; }
        GeneralUserDTO Author { get; }
        DateTime? CreateDate { get; }
    }
}
