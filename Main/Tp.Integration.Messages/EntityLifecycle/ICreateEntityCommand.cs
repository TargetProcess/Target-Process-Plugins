using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	public interface ICreateEntityCommand<out TEntityDto>
		where TEntityDto : DataTransferObject, new()
	{
		TEntityDto Dto { get; }
	}
}
