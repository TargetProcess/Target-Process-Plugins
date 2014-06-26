// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.PluginCommand;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.PluginCommand
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class PluginCommandHandlerSpecs
	{
		private TransportMock _transportMock;

		[BeforeScenario]
		public void BeforeScenario()
		{
			_transportMock = TransportMock.CreateWithoutStructureMapClear(typeof (WhenAddANewProfileSpecs).Assembly,
			                                                              typeof (WhenAddANewProfileSpecs).Assembly);

			var commands = ObjectFactory.GetInstance<IPluginCommandRepository>();

			ObjectFactory.EjectAllInstancesOf<IPluginCommandRepository>();
			ObjectFactory.Configure(x =>
			                        	{
			                        		x.For<IPluginCommandRepository>().Singleton().Use<PluginCommandMockRepository>();
			                        		x.Forward<IPluginCommandRepository, PluginCommandMockRepository>();
			                        	});

			var pluginCommandMockRepository = ObjectFactory.GetInstance<PluginCommandMockRepository>();
			foreach (var command in commands)
			{
				pluginCommandMockRepository.Add(command);
			}
		}

		[Test]
		public void ShouldExecuteCommandIfCommandMessageReceived()
		{
			@"Given plugin has command 'command1' registered
					And plugin has command 'command2' registered
				When ExecutePluginCommand command with command name 'command1' and arguments 'arg1,arg2' received
				Then command 'command1' should be executed with arguments 'arg1,arg2'"
				.Execute();
		}

		[Test]
		public void ShouldBeAbleToRetrieveAllPluginCommands()
		{
			@"Given plugin has command 'command1' registered
					And plugin has command 'command2' registered
				When getting all plugin commands
				Then the following result should be retrieved: '[""AddOrUpdateProfile"",""AddProfile"",""CheckActivityLogForErrors"",""ClearActivityLog"",""command1"",""command2"",""DeleteProfile"",""GetActivityLog"",""GetCommands"",""GetProfile"",""GetProfiles"",""PluginCommandWithException"",""SyncNow"",""TestPluginCustomCommand""]'
			"
				.Execute();
		}

		[Test]
		public void ShouldThrowExceptionIfNoCommandFound()
		{
			@"Given plugin has command 'command1' registered
					And plugin has command 'command2' registered
				When ExecutePluginCommand command with command name 'command3' and arguments 'arg1,arg2' received
				Then response message with Error status should be sent
				"
				.Execute();
		}

		[Test]
		public void ShouldThrowExceptionIfFewCommandFound()
		{
			@"Given plugin has command 'command1' registered
					And plugin has command 'command1' registered
				When ExecutePluginCommand command with command name 'command3' and arguments 'arg1,arg2' received
				Then response message with Error status should be sent
				"
				.Execute();
		}

		[Test]
		public void ShouldSendResponseWithExceptionIfExceptionInCommandOccurs()
		{
			@"Given plugin has command 'command1' which throws exception registered
				When ExecutePluginCommand command with command name 'command1' and arguments 'arg1,arg2' received
				Then response message with Error status should be sent
				"
				.Execute();
		}

		[Given("plugin has command '$commandName' which throws exception registered")]
		public void PluginContainsCommandThatThrowsException(string commandName)
		{
			RegisterCommand(new PluginCommandWithException {Name = commandName});
		}

		[Given("plugin has command '$commandName' registered")]
		public void PluginContainsCommand(string commandName)
		{
			RegisterCommand(new TestPluginCustomCommand {Name = commandName});
		}

		private static void RegisterCommand(IPluginCommand command)
		{
			var commandFactory = ObjectFactory.GetInstance<PluginCommandMockRepository>();
			commandFactory.Add(command);
		}

		[Given("ExecutePluginCommand command with command name '$commandName' and no arguments")]
		public void PluginCommandMessageCreated(string commandName)
		{
		}

		[When("ExecutePluginCommand command with command name '$commandName' and arguments '$arguments' received")]
		public void PluginCommandMessageReceived(string commandName, string arguments)
		{
			_transportMock.HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = AccountName.Empty.Value}},
				new ExecutePluginCommandCommand {CommandName = commandName, Arguments = arguments});
		}

		[When("getting all plugin commands")]
		public void GetAllPluginCommands()
		{
			_transportMock.HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = AccountName.Empty.Value}},
				new ExecutePluginCommandCommand {CommandName = EmbeddedPluginCommands.GetCommands, Arguments = string.Empty});
		}

		[Then(@"command '$commandName' should be executed with arguments '$arguments'")]
		public void CommandShouldBeExecuted(string commandName, string arguments)
		{
			var customCommandResponse = GetResponseMessage();

			var command = ObjectFactory.GetInstance<PluginCommandMockRepository>().Where(x => x.Name == commandName).Single() as
			              TestPluginCustomCommand;
			command.ResponseMessage.ResponseData.Should(Is.EqualTo(customCommandResponse.ResponseData));
			command.Arguments.Should(Is.EqualTo(arguments));
			customCommandResponse.PluginCommandStatus.Should(Is.EqualTo(PluginCommandStatus.Succeed));
		}

		private PluginCommandResponseMessage GetResponseMessage()
		{
			return _transportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
		}

		[Then(@"the following result should be retrieved: '$data'")]
		public void PluginCommandResponseMessageShouldBeSend(string data)
		{
			var customCommandResponse = GetResponseMessage();
			customCommandResponse.ResponseData.Should(Is.EquivalentTo(data));
			customCommandResponse.PluginCommandStatus.Should(Is.EqualTo(PluginCommandStatus.Succeed));
		}

		[Then("response message with Error status should be sent")]
		public void CheckExceptionWasThrown()
		{
			var customCommandResponse = GetResponseMessage();
			customCommandResponse.PluginCommandStatus.Should(Is.EqualTo(PluginCommandStatus.Error));
		}
	}

	public class PluginCommandWithException : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			throw new ApplicationException("Exception");
		}

		public PluginCommandWithException()
		{
			Name = "PluginCommandWithException";
		}

		public string Name { get; set; }
	}

	public class TestPluginCustomCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string arguments)
		{
			Arguments = arguments;
			var pluginCustomCommandResponse = new PluginCommandResponseMessage {ResponseData = "Some Response"};
			ResponseMessage = pluginCustomCommandResponse;
			return pluginCustomCommandResponse;
		}

		public PluginCommandResponseMessage ResponseMessage { get; private set; }

		public string Arguments { get; private set; }

		public TestPluginCustomCommand()
		{
			Name = "TestPluginCustomCommand";
		}

		public string Name { get; set; }
	}
}