// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.LegacyProfileConvertsion.Common
{
	public class ConvertorArgs : IConvertorArgs
	{
		public string AccountName { get; private set; }
		public string TpConnectionString { get; private set; }
		public string PluginConnectionString { get; private set; }

		public ConvertorArgs(string[] args)
		{
			var accountName = new CmdLineString("acc", false,
			                                    "Target Process Account Name. Converted Profile will be created for this account.");

			var cmdLine = new ConsoleCmdLine();
			cmdLine.RegisterParameter(accountName);

			var tpConnection = new CmdLineString("tpdb", true, "Plugin database connection string.");
			cmdLine.RegisterParameter(tpConnection);

			var pluginConnection = new CmdLineString("plugindb", false, "Target Process database connection string.");
			cmdLine.RegisterParameter(pluginConnection);
			cmdLine.Parse(args);

			TpConnectionString = tpConnection;
			AccountName = string.IsNullOrEmpty(accountName) ? Integration.Messages.AccountName.Empty.Value : accountName;

			PluginConnectionString = string.IsNullOrEmpty(pluginConnection) ? TpConnectionString : pluginConnection;
		}

		string IDatabaseConfiguration.ConnectionString
		{
			get { return PluginConnectionString; }
		}
	}
}
