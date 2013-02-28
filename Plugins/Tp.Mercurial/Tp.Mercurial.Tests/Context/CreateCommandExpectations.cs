// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Testing.Common;

namespace Tp.Mercurial.Tests.Context
{
	public class CreateCommandExpectations
	{
		private readonly Type _dtoType;
		private Func<CreateCommand, ISagaMessage> _message;
		private readonly TransportMock _transport;

		public CreateCommandExpectations(Type dtoType, TransportMock transport)
		{
			_dtoType = dtoType;
			_transport = transport;
		}

		public void Reply(Func<CreateCommand, ISagaMessage> message)
		{
			_message = message;
			_transport.On<CreateCommand>(x => _dtoType.IsAssignableFrom(x.Dto.GetType())).Reply(x => _message(x));
		}
	}
}