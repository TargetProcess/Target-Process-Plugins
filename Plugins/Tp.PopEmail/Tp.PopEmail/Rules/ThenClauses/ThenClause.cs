// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
    public abstract class ThenClause : IThenClause
    {
        protected readonly ITpBus Bus;
        private readonly IStorageRepository _storage;
        protected readonly int ProjectId;

        protected ThenClause(ParseNode clauseNode, ITpBus bus, IStorageRepository storage)
        {
            Bus = bus;
            _storage = storage;

            var projectIdNode = ClauseFactory.FindRecursive(TokenType.NUMBER, clauseNode);
            ProjectId = int.Parse(projectIdNode.Token.Text);
        }

        public virtual bool IsMatched(EmailMessage message)
        {
            var projects = _storage.Get<ProjectDTO>();
            return projects.Any(x => x.ProjectID == ProjectId && x.DeleteDate == null);
        }

        public abstract void Execute(MessageDTO dto, AttachmentDTO[] attachments, int[] requesters);
    }
}
