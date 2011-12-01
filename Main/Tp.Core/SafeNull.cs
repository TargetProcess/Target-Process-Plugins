// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Core
{
	public abstract class SafeNull<TNullObject, TNullObjectBaseInterface>
		where TNullObject : TNullObjectBaseInterface, INullable, new()
	{
		public static readonly TNullObjectBaseInterface Instance = new TNullObject();

		public bool IsNull
		{
			get { return true; }
		}
	}
}