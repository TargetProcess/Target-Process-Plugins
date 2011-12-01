// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture]
	public class CommandBusTests
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<PluginStorageWithInMemoryPersisterMockRegistry>());
		}

		[Test]
		public void ShouldConvertToCreateMessageCorrectly()
		{
			var bugDto = new BugDTO {Name = "Name"};
			ObjectFactory.GetInstance<ITpBus>().Send(new CreateBugCommand(bugDto));
			ObjectFactory.GetInstance<IBus>().AssertWasCalled(
				x => x.Send(Arg<CreateCommand[]>.Matches(y => y.Length == 1 && (((BugDTO)y[0].Dto).Name).Equals(bugDto.Name))));
		}

		[Test]
		public void ShouldConvertToUpdateMessageCorrectly()
		{
			var bugDto = new BugDTO {Name = "Name"};
			ObjectFactory.GetInstance<ITpBus>().Send(new UpdateBugCommand(bugDto, new Enum[] {BugField.Name}));
			ObjectFactory.GetInstance<IBus>().AssertWasCalled(
				x =>
				x.Send(
					Arg<UpdateCommand[]>.Matches(
						y => y.Length == 1 && ((BugDTO)y[0].Dto).Name.Equals(bugDto.Name) && y[0].ChangedFields.ToArray().Contains(BugField.Name.ToString()))));
		}

		[Test]
		public void ShouldConvertToDeleteMessageCorrectly()
		{
			const int bugId = 1;
			ObjectFactory.GetInstance<ITpBus>().Send(new DeleteBugCommand(bugId));
			ObjectFactory.GetInstance<IBus>().AssertWasCalled(
				x => x.Send(
					Arg<DeleteCommand[]>.Matches(
						y => y.Length == 1 && y[0].Id == 1 && y[0].DtoType.FullName == typeof (BugDTO).FullName)));
		}
	}
}