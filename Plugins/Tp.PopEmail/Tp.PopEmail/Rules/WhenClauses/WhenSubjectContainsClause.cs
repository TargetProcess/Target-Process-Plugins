// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.WhenClauses
{
	public class WhenSubjectContainsClause : IWhenClause
	{
		private readonly string[] _keys;

		private WhenSubjectContainsClause(string subjectSubstring)
		{
			_keys = subjectSubstring.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
		}

		public bool IsMatched(EmailMessage email)
		{
			if (string.IsNullOrEmpty(email.Subject))
			{
				return _keys.Any(x => string.IsNullOrEmpty(x.Trim()));
			}

			return _keys.Any(x => email.Subject.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
		}

		public static IWhenClause Create(ParseNode clauseNode)
		{
			var clauseParameter = ClauseFactory.FindRecursive(TokenType.STRING_PARAM, clauseNode).Token.Text.Trim('\'');
			return new WhenSubjectContainsClause(clauseParameter);
		}
	}
}