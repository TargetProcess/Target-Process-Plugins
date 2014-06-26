// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core
{
	public interface IStatelessConverter<in TSrc, out TDst, in TState>
	{
		TDst Convert(TSrc val, TState state);
	}

	public interface IStatelessConverter<in TState>
	{
		object Convert(object val, Type valType, TState state);
	}
}