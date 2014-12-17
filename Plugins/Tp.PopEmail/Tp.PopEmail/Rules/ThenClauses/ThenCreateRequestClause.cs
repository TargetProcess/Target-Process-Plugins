// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
	public class ThenCreateRequestClause : ThenClause
	{
		private ThenCreateRequestClause(ParseNode clauseNode, ITpBus bus, IStorageRepository storage)
			: base(clauseNode, bus, storage)
		{
		}

		public override void Execute(MessageDTO dto, AttachmentDTO[] attachments, int[] requesters)
		{
			var command = new CreateRequestFromMessageCommand { MessageDto = dto, ProjectId = _projectId, Attachments = attachments, Requesters = requesters };
			_bus.SendLocal(command);
		}

		public static IThenClause Create(ParseNode clauseNode)
		{
			return new ThenCreateRequestClause(clauseNode, ObjectFactory.GetInstance<ITpBus>(),
			                                   ObjectFactory.GetInstance<IStorageRepository>());
		}
	}
}
