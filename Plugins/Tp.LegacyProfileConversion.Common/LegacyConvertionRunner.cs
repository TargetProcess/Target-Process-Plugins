// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Plugin.Common.StructureMap;

namespace Tp.LegacyProfileConvertsion.Common
{
	public class LegacyConvertionRunner<TLegacyProfileConvertor, TLegacyProfile>
		where TLegacyProfileConvertor : LegacyProfileConvertorBase<TLegacyProfile>
	{
		public void Execute(string[] args)
		{
			var options = new ConvertorArgs(args);

			ObjectFactory.Configure(x =>
			                         {
			                         	x.AddRegistry<PluginRegistry>();
			                         	x.For<IConvertorArgs>().Use(options);
			                         });

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(options));

			ObjectFactory.GetInstance<TLegacyProfileConvertor>().Execute();
		}
	}
}