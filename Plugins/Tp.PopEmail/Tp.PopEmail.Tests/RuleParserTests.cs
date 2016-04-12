// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TaskCreator.Tests;
using Tp.PopEmailIntegration.Rules;

namespace Tp.PopEmailIntegration
{
	[TestFixture]
	[Category("PartPlugins0")]
	public class RuleParserTests
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Initialize(x =>
				{
					x.For<IStorageRepository>().HybridHttpOrThreadLocalScoped().Use(
						MockRepository.GenerateStub<IStorageRepository>());
					x.For<ITpBus>().HybridHttpOrThreadLocalScoped().Use<TpBusMock>();
					x.For<IActivityLogger>().HybridHttpOrThreadLocalScoped().Use(MockRepository.GenerateStub<IActivityLogger>());
					x.Forward<ITpBus, TpBusMock>();
					x.Forward<ITpBus, ICommandBus>();
					x.Forward<ITpBus, ILocalBus>();
				});
		}

		[Test]
		public void Parse()
		{
			var storageRepository = ObjectFactory.GetInstance<IStorageRepository>();
			var activityLogger = ObjectFactory.GetInstance<IActivityLogger>();

			storageRepository.Stub(x => x.GetProfile<ProjectEmailProfile>()).Return(
				new ProjectEmailProfile
					{
						Rules =
							"when company matched to project 2 then attach to project 2 and create private request in project 1 and attach request to team 100 and attach request to team 101"
					});

			var ruleParser = new RuleParser(storageRepository, activityLogger);
			IEnumerable<MailRule> mailRules = ruleParser.Parse().ToArray();

			Assert.NotNull(mailRules);
			Assert.AreEqual(1, mailRules.Count());
		}
	}
}