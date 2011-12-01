// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Rules.WhenClauses
{
	public interface IWhenClause
	{
		bool IsMatched(EmailMessage email);
	}
}