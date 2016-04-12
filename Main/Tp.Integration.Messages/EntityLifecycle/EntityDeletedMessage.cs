using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Base message indicating that some entity was deleted in TargetProcess.
	/// </summary>
	/// <typeparam name="TEntityDto">The type of deleted entity.</typeparam>
	[Serializable]
	public class EntityDeletedMessage<TEntityDto> : EntityMessage<TEntityDto>, IEntityDeletedMessage<TEntityDto>
		where TEntityDto : DataTransferObject, new()
	{
	}
}
