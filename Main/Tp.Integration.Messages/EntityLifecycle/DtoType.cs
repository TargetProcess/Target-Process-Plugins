// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages.EntityLifecycle
{
	[Serializable]
	public class DtoType
	{
		public DtoType()
		{
		}

		public DtoType(Type type)
		{
			FullName = type.FullName;
		}

		public new Type GetType()
		{
			return Type.GetType(FullName);
		}

		public string FullName { get; set; }
	}
}