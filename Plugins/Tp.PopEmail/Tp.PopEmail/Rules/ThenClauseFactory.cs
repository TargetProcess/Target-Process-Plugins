﻿// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.PopEmailIntegration.Rules.Parsing;
using Tp.PopEmailIntegration.Rules.ThenClauses;

namespace Tp.PopEmailIntegration.Rules
{
    internal class ThenClauseFactory : ClauseFactory
    {
        public ThenClauseFactory()
        {
            _thenClauses[TokenType.AttachToProjectClause] = ThenAttachToProjectClause.Create;
            _thenClauses[TokenType.CreateRequestClause] = ThenCreateRequestClause.CreatePrivateRequest;
            _thenClauses[TokenType.CreatePrivateRequestClause] = ThenCreateRequestClause.CreatePrivateRequest;
            _thenClauses[TokenType.CreatePublicRequestClause] = ThenCreateRequestClause.CreatePublicRequest;
        }

        public ThenClauseComposite CreateBy(ParseNode clauseSubtree)
        {
            var result = new ThenClauseComposite();
            foreach (var whenClause in _thenClauses.Keys)
            {
                var clause = FindRecursive(whenClause, clauseSubtree.Nodes.ToArray());
                if (clause == null)
                {
                    continue;
                }

                result.Add(_thenClauses[whenClause](clause));
            }
            return result;
        }

        private readonly Dictionary<TokenType, Func<ParseNode, IThenClause>>
            _thenClauses = new Dictionary<TokenType, Func<ParseNode, IThenClause>>();
    }
}
