// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text;
using Tp.Integration.Plugin.Common;

namespace Tp.Subversion.StructureMap
{
	public class SvnPluginExcludedAssemblies : List<string>, IExcludedAssemblyNamesSource
	{
		public SvnPluginExcludedAssemblies()
		{
			AddRange(new[] { "SharpSvn.dll", "SharpSvn-DB44-20-Win32.dll", "SharpSvn-SASL21-23-Win32.dll", "vcredist_x86.exe" });
		}
	}
}