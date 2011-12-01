// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.LegacyProfileConvertsion.Common;

namespace Tp.PopEmailIntegration.LegacyProfileConversion
{
	public class Program
	{
		public static void Main(string[] args)
		{
			new LegacyConvertionRunner<LegacyProfileConvertor, Project[]>().Execute(args);
		}
	}
}
