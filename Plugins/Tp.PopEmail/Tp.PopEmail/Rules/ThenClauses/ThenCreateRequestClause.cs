// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
    public class ThenCreateRequestClause : ThenClause
    {
        private readonly bool _isPrivate;
        private readonly int? _squadId;

        private ThenCreateRequestClause(ParseNode clauseNode, ITpBus bus, IStorageRepository storage, bool isPrivate = false)
            : base(clauseNode, bus, storage)
        {
            var attachToRequestPart = ClauseFactory.FindRecursive(TokenType.AttachRequestToTeamPart, clauseNode.Parent);
            if (attachToRequestPart != null)
            {
                foreach (var squadIdNode in from node in attachToRequestPart.Nodes
                    where node.Token.Type == TokenType.AttachRequestToTeamClause
                    select ClauseFactory.FindRecursive(TokenType.NUMBER, node))
                {
                    _squadId = System.Int32.Parse(squadIdNode.Token.Text);
                    break;
                }
            }

            _isPrivate = isPrivate;
        }

        public override void Execute(MessageDTO dto, AttachmentDTO[] attachments, int[] requesters)
        {
            var command = new CreateRequestFromMessageCommand
            {
                MessageDto = dto,
                ProjectId = _projectId,
                Attachments = attachments,
                Requesters = requesters,
                IsPrivate = _isPrivate,
                SquadId = _squadId
            };
            _bus.SendLocal(command);
        }

        public static IThenClause CreatePublicRequest(ParseNode clauseNode)
        {
            return new ThenCreateRequestClause(clauseNode, ObjectFactory.GetInstance<ITpBus>(),
                ObjectFactory.GetInstance<IStorageRepository>());
        }

        public static IThenClause CreatePrivateRequest(ParseNode clauseNode)
        {
            return new ThenCreateRequestClause(clauseNode, ObjectFactory.GetInstance<ITpBus>(),
                ObjectFactory.GetInstance<IStorageRepository>(), true);
        }
    }
}
