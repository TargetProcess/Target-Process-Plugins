// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Rules.WhenClauses
{
    public class WhenClauseComposite
    {
        private readonly List<IWhenClause> _whenClauses = new List<IWhenClause>();

        public void Add(IWhenClause clause)
        {
            _whenClauses.Add(clause);
        }

        public bool IsMatched(EmailMessage emailMessage)
        {
            return _whenClauses.All(whenClause => whenClause.IsMatched(emailMessage));
        }
    }
}
