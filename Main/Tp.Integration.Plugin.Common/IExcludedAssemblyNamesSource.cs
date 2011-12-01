// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// Put this interface implementation in StructureMap in order to exclude some assemblies from scanning by NserviceBus.
	/// This can be helpful if you use some non-.Net libraries in plugin.
	/// </summary>
	public interface IExcludedAssemblyNamesSource : IEnumerable<string>
	{
	}
}