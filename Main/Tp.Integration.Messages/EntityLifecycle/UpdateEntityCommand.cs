using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
    /// <summary>
    /// Base command for updating entity in TargetProcess.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of entity to create.</typeparam>
    [Serializable]
    public abstract class UpdateEntityCommand<TEntityDto> : IUpdateEntityCommand<TEntityDto>
        where TEntityDto : DataTransferObject, new()
    {
        protected UpdateEntityCommand(TEntityDto dto) : this(dto, Array.Empty<Enum>())
        {
        }

        protected UpdateEntityCommand(TEntityDto dto, Enum[] changedFields)
        {
            ChangedFields = changedFields;
            Dto = dto;
        }

        public TEntityDto Dto { get; set; }
        public Enum[] ChangedFields { get; set; }
    }

    public interface IUpdateEntityCommand<out TEntityDto>
        where TEntityDto : DataTransferObject, new()
    {
        TEntityDto Dto { get; }
        Enum[] ChangedFields { get; }
    }
}
