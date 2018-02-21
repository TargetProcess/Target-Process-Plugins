// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
    public class ThenAttachToProjectClause : ThenClause
    {
        public ThenAttachToProjectClause(ParseNode clauseNode, ITpBus bus, IStorageRepository storage)
            : base(clauseNode, bus, storage)
        {
        }

        public override void Execute(MessageDTO dto, AttachmentDTO[] attachments, int[] requesters)
        {
            _bus.SendLocal(new AttachMessageToProjectCommand { MessageDto = dto, ProjectId = _projectId });
        }

        public static IThenClause Create(ParseNode clauseNode)
        {
            return new ThenAttachToProjectClause(clauseNode, ObjectFactory.GetInstance<ITpBus>(),
                ObjectFactory.GetInstance<IStorageRepository>());
        }
    }

    [Serializable]
    public class AttachMessageToProjectCommand : IPluginLocalMessage
    {
        public MessageDTO MessageDto { get; set; }
        public int ProjectId { get; set; }
    }
}
