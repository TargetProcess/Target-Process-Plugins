// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
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
		public MailRule(WhenClauseComposite whenClause, ThenClauseComposite thenClause)
		{
			_whenClause = whenClause;
			_thenClause = thenClause;
		}

		private readonly WhenClauseComposite _whenClause;
		private readonly ThenClauseComposite _thenClause;

		public bool IsMatched(EmailMessage message)
		{
			return _whenClause.IsMatched(message) && _thenClause.IsMatched(message);
		}

		public void Execute(MessageDTO dto, AttachmentDTO[] attachments)
		{
			_thenClause.Execute(dto, attachments);
		}

		public bool IsNull
		{
			get { return false; }
		}
	}
}