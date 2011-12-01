// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using StructureMap;
using Tp.Integration.Common;
using Tp.PopEmailIntegration.Sagas;

namespace Tp.PopEmailIntegration
{
	public class UpdateMessageBodySagaContext : PopEmailIntegrationContext
	{
		public UpdateMessageBodySagaContext()
		{
			ObjectFactory.Configure(x => x.For<UpdateMessageBodySagaContext>().Use(this));

			_command = new UpdateMessageBodyCommandInternal {AttachmentDtos = new AttachmentDTO[] {}, MessageDto = new MessageDTO(), OuterSagaId = OuterSagaId};
		}

		private readonly UpdateMessageBodyCommandInternal _command;
		private readonly Guid _outerSagaId = Guid.NewGuid();


		public UpdateMessageBodyCommandInternal Command
		{
			get { return _command; }
		}

		public Guid OuterSagaId
		{
			get { return _outerSagaId; }
		}
	}
}