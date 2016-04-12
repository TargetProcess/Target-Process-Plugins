// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.StructureMap;

namespace Tp.LegacyProfileConvertsion.Common.StructureMap
{
	public class LegacyProfileConversionRegistry : PluginRegistry
	{
		protected override ITpBus CreateTpBus()
		{
			return TpBusSafeNull.Instance;
		}
	}
}