// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Reflection;
using Tp.LegacyProfileConversion.Common.Testing;

namespace Tp.Subversion.LegacyProfileConversionFeature
{
	public class SubversionLegacyProfileConversionUnitTestRegistry : LegacyProfileConverterUnitTestRegistry
	{
		protected override Assembly PluginAssembly
		{
			get { return typeof (SubversionPluginProfile).Assembly; }
		}
	}
}