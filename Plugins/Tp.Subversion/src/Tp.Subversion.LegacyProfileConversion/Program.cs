//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.LegacyProfileConvertsion.Common;
using Tp.Subversion.StructureMap;

namespace Tp.Subversion.LegacyProfileConversion
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ObjectFactory.Initialize(x => x.AddRegistry<SubversionRegistry>());
			new LegacyConvertionRunner<LegacyProfileConvertor, PluginProfile>().Execute(args);
		}
	}
}