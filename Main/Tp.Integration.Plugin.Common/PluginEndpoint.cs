// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Transactions;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus.Serialization;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router;
using Tp.Integration.Messages.ServiceBus.Transport.UiPriority;
using Tp.Integration.Messages.ServiceBus.UnicastBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Integration.Plugin.Common.Properties;
using log4net.Config;

namespace Tp.Integration.Plugin.Common
{
	public class PluginEndpoint : IConfigureThisEndpoint, IWantCustomInitialization
	{
		public void Init()
		{
			SetLoggingLibrary.Log4Net(XmlConfigurator.Configure);

			ObjectFactory.Initialize(x => x.Scan(y =>
			                                     	{
														var assembliesToSkip = AssembliesToSkip().Select(Path.GetFileNameWithoutExtension);
			                                     		y.AssembliesFromApplicationBaseDirectory(assembly => !assembliesToSkip.Contains(assembly.GetName().Name));
			                                     		y.LookForRegistries();
			                                     	}));

			var config = Configure.With(AssembliesToScan())
				.CustomConfigurationSource(new PluginConfigurationSource())
				.StructureMapBuilder(ObjectFactory.Container)
				.AdvancedXmlSerializer();

			if (Transport == Transport.UiPriority)
			{
				config = config.MsmqTransport<MsmqUiPriorityTransport>().IsTransactional(IsTransactional).IsolationLevel(IsolationLevel.ReadUncommitted)
					.PurgeOnStartup(false);
			}
			else if (Transport == Transport.Routable)
			{
				config = config.MsmqTransport<MsmqRoutableTransport>().IsTransactional(IsTransactional).IsolationLevel(IsolationLevel.ReadUncommitted)
					.HostingMode(RoutableTransportMode)
					.PurgeOnStartup(false);
			}

			var bus = config.Sagas()
				.TpDatabaseSagaPersister()
				.TpUnicastBus()
				//.ImpersonateSender(true)
				.LoadMessageHandlers(GetHandlersOrder()).CreateBus();

			bus.Start();
		}

		private static RoutableTransportMode RoutableTransportMode
		{
			get { return (RoutableTransportMode) Enum.Parse(typeof (RoutableTransportMode), Settings.Default.RoutableTransportMode); }
		}

		private static Transport Transport
		{
			get { return (Transport) Enum.Parse(typeof (Transport), Settings.Default.Transport); }
		}

		protected bool IsTransactional
		{
			get { return Settings.Default.IsTransactional; }
		}

		private static IEnumerable<Assembly> AssembliesToScan()
		{
			var result = AllAssemblies.Except(AssembliesToSkip()[0]);
			AssembliesToSkip().Skip(1).ForEach(x => result.And(x));
			var excludedAssemblyNames = ObjectFactory.TryGetInstance<IExcludedAssemblyNamesSource>();
			if (excludedAssemblyNames != null)
			{
				result = excludedAssemblyNames.Aggregate(result,
				                                         (current, excludedAssemblyName) => current.And(excludedAssemblyName));
			}

			return result.ToArray();
		}

		private static string[] AssembliesToSkip()
		{
			return new[] { "PluginUninstallUtil.exe", "Tp.LegacyProfileConversion.Common.dll" };
		}

		private static First<PluginGateway> GetHandlersOrder()
		{
			var ordering = First<PluginGateway>.Then<PluginGateway>();
			var messageHandlerOrdering = ObjectFactory.TryGetInstance<ICustomPluginSpecifyMessageHandlerOrdering>();
			if (messageHandlerOrdering != null)
			{
				messageHandlerOrdering.SpecifyHandlersOrder(ordering);
			}

			return ordering;
		}

		#region PluginConfigurationSource

		public class PluginConfigurationSource : IConfigurationSource
		{
			public T GetConfiguration<T>() where T : class
			{
				{
					if (typeof (T) == typeof (MsmqTransportConfig))
					{
						return new MsmqTransportConfig
						       	{
						       		ErrorQueue = "Tp.Error",
						       		InputQueue = ObjectFactory.GetInstance<IPluginSettings>().PluginInputQueue,
						       		MaxRetries = 1,
						       		NumberOfWorkerThreads = 1
						       	} as T;
					}

					if (typeof (T) == typeof (UnicastBusConfig))
					{
						var mappings = new MessageEndpointMappingCollection
						               	{
						               		new MessageEndpointMapping
						               			{Messages = "Tp.Integration.Messages", Endpoint = TargetProcessInputQueue}
						               	};
						foreach (var mapping in GetExtraMappings())
						{
							mappings.Add(mapping);
						}

						return new UnicastBusConfig
						       	{
						       		MessageEndpointMappings = mappings
						       	} as T;
					}

					return null;
				}
			}

			protected string TargetProcessInputQueue
			{
				get { return Settings.Default.TargetProcessInputQueue; }
			}

			protected virtual IEnumerable<MessageEndpointMapping> GetExtraMappings()
			{
				return new MessageEndpointMapping[] {};
			}

			public static PluginInfo PluginInfo
			{
				get
				{
					var pluginAssemblyAttribute = ObjectFactory.GetInstance<IPluginMetadata>().PluginData;
					return new PluginInfo
					       	{
					       		Name = pluginAssemblyAttribute.Name,
					       		Description = pluginAssemblyAttribute.Description,
					       		Category = pluginAssemblyAttribute.Category
					       	};
				}
			}
		}

		#endregion
	}
}