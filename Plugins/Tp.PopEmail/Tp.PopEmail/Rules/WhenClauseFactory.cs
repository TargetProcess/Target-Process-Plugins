// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.PopEmailIntegration.Rules.Parsing;
using Tp.PopEmailIntegration.Rules.ThenClauses;
using Tp.PopEmailIntegration.Rules.WhenClauses;

namespace Tp.PopEmailIntegration.Rules
{
    internal class WhenClauseFactory : ClauseFactory
    {
        private readonly Dictionary<TokenType, Func<ParseNode, IWhenClause>> _whenClauses =
            new Dictionary<TokenType, Func<ParseNode, IWhenClause>>();

        public WhenClauseFactory(IThenClause thenClause)
        {
            _whenClauses[TokenType.SubjectContainsClause] = WhenSubjectContainsClause.Create;

            _whenClauses[TokenType.CompanyMatchedClause] = WhenSenderAndProjectCompanyMatched.Create;
        }

        public WhenClauseComposite CreateBy(ParseNode clauseSubtree)
        {
            var result = new WhenClauseComposite();
            foreach (var whenClause in _whenClauses.Keys)
            {
                var clause = FindRecursive(whenClause, clauseSubtree.Nodes.ToArray());
                if (clause == null)
                {
                    continue;
                }

                result.Add(_whenClauses[whenClause](clause));
            }
            return result;
        }
    }
}
