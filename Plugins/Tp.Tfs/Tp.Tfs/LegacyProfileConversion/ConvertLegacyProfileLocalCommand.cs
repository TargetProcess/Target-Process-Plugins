// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.LegacyProfileConvertsion.Common;
using log4net;
using PluginProfile = Tp.LegacyProfileConvertsion.Common.PluginProfile;

namespace Tp.Tfs.LegacyProfileConversion
{
	[Serializable]
	public class ConvertLegacyProfileLocalMessage : IPluginLocalMessage
	{
		public string LegacyProfileName { get; set; }
	}

	public class ConvertLegacyProfileHandler : IHandleMessages<ConvertLegacyProfileLocalMessage>
	{
		private readonly ILog _log;

		public ConvertLegacyProfileHandler(ILogManager logManager)
		{
			_log = logManager.GetLogger(GetType());
		}

		public void Handle(ConvertLegacyProfileLocalMessage message)
		{
			var profileName = message.LegacyProfileName;
			var parameters = GetParams(message);
			_log.Info("TFS legacy profile converter parsed parameters");

			var runner = new LegacyConvertionRunner<LegacyProfileConvertor, PluginProfile>();
			runner.InitRunner(new Integration.Plugin.Common.StructureMap.PluginRegistry(), parameters.ToArray());
			_log.Info("TFS legacy profile converter inited db connections and structure map");

			var converter = ObjectFactory.GetInstance<LegacyProfileConvertor>();
			converter.Execute(profileName);
			_log.Info("TFS legacy profile converter executed conversion");
		}

		private static IEnumerable<string> GetParams(ConvertLegacyProfileLocalMessage message)
		{
			var pluginDb = ObjectFactory.GetInstance<IDatabaseConfiguration>().ConnectionString;

			yield return "-plugindb";
			yield return pluginDb;

			var tpConnectionString = String.Empty;
			var accountName = ObjectFactory.GetInstance<IPluginContext>().AccountName;
			if (accountName != AccountName.Empty)
			{
				var builder = new SqlConnectionStringBuilder(pluginDb) { InitialCatalog = accountName.Value.ToLower().Replace(".tpondemand.com", String.Empty) };

				tpConnectionString += builder.ConnectionString;
				yield return "-acc";
				yield return accountName.Value;
			}
			else
			{
				tpConnectionString += pluginDb;
			}

			yield return "-tpdb";
			yield return tpConnectionString;
		}
	}
}