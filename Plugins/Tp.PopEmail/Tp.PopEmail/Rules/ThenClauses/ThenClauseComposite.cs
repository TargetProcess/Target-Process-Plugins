// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
	public class ThenClauseComposite : IThenClause
	{
		private readonly List<IThenClause> _clauses = new List<IThenClause>();

		public void Add(IThenClause thenClause)
		{
			_clauses.Add(thenClause);
		}

		public void Execute(MessageDTO dto, AttachmentDTO[] attachments, int[] requesters)
		{
			foreach (var thenClause in _clauses)
			{
				thenClause.Execute(dto, attachments, requesters);
			}
		}

		public bool IsMatched(EmailMessage message)
		{
			return _clauses.Any(x => x.IsMatched(message));
		}
	}
}