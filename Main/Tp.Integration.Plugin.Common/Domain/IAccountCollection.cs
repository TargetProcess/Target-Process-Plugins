// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	public interface IAccountCollection : IEnumerable<IAccountReadonly>
	{
		IAccount GetOrCreate(AccountName accountName);
		void Remove(AccountName accountName);
	}
}