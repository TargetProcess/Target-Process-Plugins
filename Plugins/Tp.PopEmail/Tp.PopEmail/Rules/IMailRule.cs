// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Core;
using Tp.Integration.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Rules
{
    public interface IMailRule : INullable
    {
        bool IsMatched(EmailMessage message);
        void Execute(EmailMessage emailMessage, MessageDTO dto, AttachmentDTO[] attachments);
    }

    public class MailRuleSafeNull : SafeNull<MailRuleSafeNull, IMailRule>, IMailRule
    {
        public bool IsMatched(EmailMessage message)
        {
            return false;
        }

        public void Execute(EmailMessage emailMessage, MessageDTO dto, AttachmentDTO[] attachments)
        {
        }
    }
}
