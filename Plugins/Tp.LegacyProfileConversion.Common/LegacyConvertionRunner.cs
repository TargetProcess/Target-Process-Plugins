// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Data.SqlClient;
using StructureMap;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.LegacyProfileConvertsion.Common.StructureMap;

namespace Tp.LegacyProfileConvertsion.Common
{
	public class LegacyConvertionRunner<TLegacyProfileConvertor, TLegacyProfile>
		where TLegacyProfileConvertor : LegacyProfileConvertorBase<TLegacyProfile>
	{
		public void Execute(string[] args)
		{
			InitRunner(new LegacyProfileConversionRegistry(), args);

			ObjectFactory.GetInstance<TLegacyProfileConvertor>().Execute();
		}

		public void InitRunner(Registry registry, string[] args)
		{
			var options = new ConvertorArgs(args);
			TestDBConnection(options.PluginConnectionString);
			TestDBConnection(options.TpConnectionString);

			ObjectFactory.Configure(x =>
			                        	{
			                        		x.AddRegistry(registry);
			                        		x.For<IConvertorArgs>().Use(options);
			                        	});

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(options));
		}


		private void TestDBConnection(string connectionString)
		{
			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					connection.Open();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}