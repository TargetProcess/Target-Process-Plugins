// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
	public interface IThenClause
	{
		void Execute(MessageDTO dto, AttachmentDTO[] attachments);
		bool IsMatched(EmailMessage message);
	}
}