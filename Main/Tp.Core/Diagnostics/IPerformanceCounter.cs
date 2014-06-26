// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core.Diagnostics
{
	public interface IPerformanceCounter : IDisposable
	{
		void Increment();
		void IncrementBy(long value);
		void Decrement();
		long RawValue { get; set; }
	}
}