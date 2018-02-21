// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Rules.ThenClauses;
using Tp.PopEmailIntegration.Rules.WhenClauses;

namespace Tp.PopEmailIntegration.Rules
{
    public class MailRule : IMailRule
    {
        public MailRule(WhenClauseComposite whenClause, ThenClauseComposite thenClause, string ruleLine)
        {
            _whenClause = whenClause;
            _thenClause = thenClause;
            _ruleLine = ruleLine;
        }

        private readonly WhenClauseComposite _whenClause;
        private readonly ThenClauseComposite _thenClause;
        private readonly string _ruleLine;

        public bool IsMatched(EmailMessage message)
        {
            return _whenClause.IsMatched(message) && _thenClause.IsMatched(message);
        }

        public void Execute(MessageDTO dto, AttachmentDTO[] attachments, int[] requesters)
        {
            _thenClause.Execute(dto, attachments, requesters);
        }

        public bool IsNull => false;

        public override string ToString()
        {
            return _ruleLine;
        }
    }
}
