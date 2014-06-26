using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Integration.Plugin.TaskCreator.Tests;
using Tp.Search.Tests.Log;

namespace Tp.Search.Tests.Registry
{
	class TestRegistry : global::StructureMap.Configuration.DSL.Registry
	{
		public TestRegistry()
		{
			For<AccountName>().Singleton().Use(new AccountName("TestAccount"));
			For<IProfileReadonly>().Use(new SearchProfile()
				{
					Initialized = false
				});
			For<ProfileName>().Use(c => c.GetInstance<IProfileReadonly>().Name);
			For<IActivityLogger>().Use<ConsoleLogger>();
			For<IActivityLoggerFactory>().Use<MockLoggerFactory>();
			For<PluginName>().Singleton().Use(new PluginName("Search"));
			For<ILocalBus>().Use<TpBusMock>();
			For<IPluginContext>().Singleton().Use(c => new PluginContextMock
				{
					AccountName = c.GetInstance<AccountName>(),
					PluginName = c.GetInstance<PluginName>(),
					ProfileName = c.GetInstance<ProfileName>()
				});
		}
	}

	internal class MockLoggerFactory : IActivityLoggerFactory
	{
		public IActivityLogger Create(IPluginContextSnapshot contextSnapshot)
		{
			return new ConsoleLogger();
		}
	}
}