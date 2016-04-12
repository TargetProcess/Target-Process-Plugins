using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	[Serializable]
	public class EntityMessage<TEntityDto> : SagaMessage
		where TEntityDto : DataTransferObject, new()
	{
		public EntityMessage()
		{
			Dto = new TEntityDto();
		}

		public TEntityDto Dto { get; set; }
		public GeneralUserDTO Author { get; set; }
		public DateTime? CreateDate { get; set; }
	}
}
