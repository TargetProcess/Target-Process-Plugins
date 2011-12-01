// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Testing.Common
{
	public abstract class CommandExpectationBase<TDto>
		where TDto : DataTransferObject, new()
	{
		protected Func<TDto, ISagaMessage> _message;
		protected TransportMock _transport;

		public void InitTransport(TransportMock transport)
		{
			_transport = transport;
		}

		public void Reply(Func<TDto, ISagaMessage> message)
		{
			_message = message;
			ReplyOnCommand();
		}

		protected abstract void ReplyOnCommand();
	}

	public class CreateCommandExpectations<TDto> : CommandExpectationBase<TDto> where TDto : DataTransferObject, new()
	{
		protected override void ReplyOnCommand()
		{
			_transport.On<CreateCommand>(x => typeof(TDto).IsAssignableFrom(x.Dto.GetType())).Reply(x => _message(x.Dto as TDto));
		}
	}

	public class UpdateCommandExpectations<TDto> : CommandExpectationBase<TDto>
		where TDto : DataTransferObject, new()
	{
		protected override void ReplyOnCommand()
		{
			_transport.On<UpdateCommand>(x => typeof(TDto).IsAssignableFrom(x.Dto.GetType())).Reply(x => _message(x.Dto as TDto));
		}
	}

	public class DeleteCommandExpectations<TDto> : CommandExpectationBase<TDto>
		where TDto : DataTransferObject, new()
	{
		protected override void ReplyOnCommand()
		{
			_transport.On<DeleteCommand>(x => typeof(TDto).IsAssignableFrom(x.DtoType.GetType())).Reply(
				x => _message(new TDto { ID = x.Id }));
		}
	}
}
