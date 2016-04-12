// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Core;

namespace Tp.Integration.Plugin.Common.FileStorage
{
	public class ProfileFileStorageSafeNull : SafeNull<ProfileFileStorageSafeNull, IProfileFileStorage>,
	                                          IProfileFileStorage, INullable
	{
		public string GetFolder()
		{
			return string.Empty;
		}

		public void Clear()
		{
		}
	}
}