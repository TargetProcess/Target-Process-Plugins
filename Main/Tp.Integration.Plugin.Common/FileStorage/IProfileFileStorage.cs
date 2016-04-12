// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Plugin.Common.FileStorage
{
	public interface IProfileFileStorage
	{
		string GetFolder();

		void Clear();
	}
}