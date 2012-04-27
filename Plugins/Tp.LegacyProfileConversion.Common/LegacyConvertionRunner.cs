// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
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
		private readonly IDictionary<string, Action> _actionsRegistry = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

		public LegacyConvertionRunner()
		{
			_actionsRegistry[ConvertorArgs.DefaultAction] = () => ObjectFactory.GetInstance<TLegacyProfileConvertor>().Execute();
		}

		public void Execute(string[] args)
		{
			InitRunner(new LegacyProfileConversionRegistry(), args);

			var action = GetAction();

			_actionsRegistry[action].Invoke();
		}

		public void AddAction(string key, Action action)
		{
			if (_actionsRegistry.ContainsKey(key))
			{
				throw new InvalidOperationException("Cannot replace already existing action");
			}

			_actionsRegistry[key] = action;
		}

		private string GetAction()
		{
			var action = ObjectFactory.GetInstance<IConvertorArgs>().Action;

			if (!_actionsRegistry.ContainsKey(action))
			{
				throw new KeyNotFoundException("Unknown action");
			}

			return action;
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