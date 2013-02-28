// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Xml;
using StructureMap;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.Tfs
{
	public static class ConfigHelper
	{
		public static bool GetWorkItemsState()
		{
			var log = ObjectFactory.GetInstance<ILogManager>().GetLogger(typeof(ConfigHelper));

			var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PluginSettings.config");

			if (!File.Exists(fullPath))
			{
				log.WarnFormat("Failed to find PluginSettings.config file by path '{0}'.", fullPath);
				return false;
			}

			var pluginConfig = new XmlDocument();
			pluginConfig.Load(fullPath);

			var workItemsStateNode = pluginConfig.DocumentElement.SelectSingleNode("setting[@name='WorkItemsState']/value");
			if (workItemsStateNode == null)
			{
				log.Warn("Failed to find PluginInputQueue setting in PluginSettings.config file.");
				return false;
			}

			bool state;
			if (!bool.TryParse(workItemsStateNode.InnerText, out state))
			{
				log.Warn("Failed to read WorkItemsState value in PluginSettings.config file.");
				return false;
			}

			return state;
		}
	}
}
