// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
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

        public IEnumerable<int> GetRequestersForEmail(EmailMessage emailMessage)
        {
            var userRepository = ObjectFactory.GetInstance<UserRepository>();
            var isCompanyMatched = _whenClauses.Any(wc => wc is WhenSenderAndProjectCompanyMatched);
            var requesterAddresses = new List<string> { emailMessage.FromAddress };
            if (emailMessage.ReplyTo != null)
            {
                requesterAddresses.AddRange(emailMessage.ReplyTo.Select(replyToAddress => replyToAddress.Address));
            }
            if (emailMessage.CC != null)
            {
                requesterAddresses.AddRange(emailMessage.CC.Select(ccAddress => ccAddress.Address));
            }

            foreach (var requesterAddress in requesterAddresses.Distinct())
            {
                var users = userRepository.GetByEmail(requesterAddress);

                var user = isCompanyMatched && string.Compare(emailMessage.FromAddress, requesterAddress, StringComparison.OrdinalIgnoreCase) == 0
                    ? users.OrderBy(x => x.UserType).FirstOrDefault(x => !x.IsDeleted)
                    : users.OrderByDescending(x => x.UserType).FirstOrDefault(x => !x.IsDeleted);

                if (user?.Id != null)
                {
                    yield return user.Id.Value;
                }
            }
        }
    }
}
