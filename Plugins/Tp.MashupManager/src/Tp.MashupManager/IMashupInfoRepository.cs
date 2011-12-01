// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager
{
	public interface IMashupInfoRepository
	{
		PluginProfileErrorCollection Add(MashupDto dto);
		PluginProfileErrorCollection Update(UpdateMashupDto dto);
		PluginProfileErrorCollection Delete(string mashupName);
	}
}