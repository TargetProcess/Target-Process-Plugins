// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.IO;
using System.Messaging;
using System.Xml;
using NServiceBus.Unicast;
using NServiceBus.Utils;

namespace Tp.Integration.Plugin.UninstallUtil
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine(
					"Please specify two parameters. First : SubscriptionStorage queue full path. Second : Plugin queue full path.");
				return;
			}

			var subscriptionStorageQueue = args[0];
			string pluginInputQueue;

			if (args.Length == 1)
			{
				pluginInputQueue = LoadPluginQueueFromConfigFile();
				if (pluginInputQueue == null)
					return;
			}
			else
			{
				pluginInputQueue = args[1];
			}

			DeletePluginQueue(pluginInputQueue);
			DeletePluginQueue(UnicastBus.GetUiQueueName(pluginInputQueue));
			ClearSubscriptionStorage(subscriptionStorageQueue, pluginInputQueue);
		}

		private static string LoadPluginQueueFromConfigFile()
		{
			if (!File.Exists("PluginSettings.config"))
			{
				Console.WriteLine(
					"Failed to find PluginSettings.config file in current directory. Specify queues by parameters instead.");
				return null;
			}

			var pluginConfig = new XmlDocument();
			pluginConfig.Load("PluginSettings.config");


			var pluginInputQueueNode = pluginConfig.DocumentElement.SelectSingleNode("setting[@name='PluginInputQueue']/value");
			if (pluginInputQueueNode == null)
			{
				Console.WriteLine("Failed to find PluginInputQueue setting in PluginSettings.config file.");
				return null;
			}

			return pluginInputQueueNode.InnerText;
		}

		private static void ClearSubscriptionStorage(string subscriptionStorageQueue, string pluginInputQueue)
		{
			var fullName = MsmqUtilities.GetFullPathWithoutPrefix(subscriptionStorageQueue);
			if (!MessageQueue.Exists(fullName)) return;

			var queue = new MessageQueue(fullName);
			foreach (var message in queue.GetAllMessages())
			{
				if (message.Label.ToLower().Contains(pluginInputQueue.ToLower()))
				{
					queue.ReceiveById(message.Id);
				}
			}
			Console.WriteLine("SubscriptionStorage queue {0} successfully cleared", subscriptionStorageQueue);
		}

		private static void DeletePluginQueue(string pluginInputQueue)
		{
			var fullName = MsmqUtilities.GetFullPathWithoutPrefix(pluginInputQueue);
			if (MessageQueue.Exists(fullName))
			{
				MessageQueue.Delete(fullName);
				Console.WriteLine("Plugin queue {0} successfully removed", fullName);
			}
			else
			{
				Console.WriteLine("Could not find Plugin queue {0}", fullName);
			}
		}
	}
}