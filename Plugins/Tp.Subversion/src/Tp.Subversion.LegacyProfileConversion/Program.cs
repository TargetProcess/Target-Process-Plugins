//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.LegacyProfileConvertsion.Common;
using Tp.Subversion.StructureMap;

namespace Tp.Subversion.LegacyProfileConversion
{
	public class Program
	{
		public static void Main(string[] args)
		{
			OutputArguments(args);
			ObjectFactory.Initialize(x => x.AddRegistry<SubversionRegistry>());
			new LegacyConvertionRunner<LegacyProfileConvertor, PluginProfile>().Execute(args);
		}
		private static void OutputArguments(IEnumerable<string> args)
		{
			foreach (var arg in args)
			{
				Console.Write(!arg.Contains(" ") ? "{0}" : "\"{0}\"", arg);
				Console.Write(" ");
			}
			Console.WriteLine();
		}
	}
}