// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTpEventCommand : CreateEntityCommand<TpEventDTO>
	{
		public CreateTpEventCommand(TpEventDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTpEventCommand : UpdateEntityCommand<TpEventDTO>
	{
		public UpdateTpEventCommand(TpEventDTO dto) : base(dto)
		{
		}

		public UpdateTpEventCommand(TpEventDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTpEventCommand : DeleteEntityCommand<TpEventDTO>
	{
		public DeleteTpEventCommand(int id) : base(id)
		{
		}
	}

}