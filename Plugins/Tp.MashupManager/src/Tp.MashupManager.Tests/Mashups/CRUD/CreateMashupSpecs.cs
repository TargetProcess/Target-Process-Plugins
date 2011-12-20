// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.MashupManager.CustomCommands;
using Tp.Testing.Common.NBehave;

namespace Tp.MashupManager.Tests.Mashups.CRUD
{
	[ActionSteps, TestFixture]
	public class CreateMashupSpecs : MashupManagerTestBase
	{
		[Test]
		public void ShouldCreateMashupOnAddMashupCommand()
		{
			@"
				Given profile created
				When handle AddMashupCommand command with args '{""Name"":""mashup1"", ""Placeholders"":""Default"", ""Script"":""alert(123)""}'
				Then 1 mashups should be sent to TP
					And mashup 'mashup1' with placeholders 'Default' and script 'alert(123)' should be sent to TP
					And 1 mashup should be in profile storage
					And mashup 'mashup1' with placeholders 'Default' and script 'alert(123)' should be in profile storage
			".Execute();
		}

		[Test]
		public void ShouldCreateMashupForAccount()
		{
			@"
				Given profile created for account 'Account1'
				When handle AddMashupCommand command with args '{""Name"":""mashup1"", ""Placeholders"":""Default"", ""Script"":""alert(123)""}'
				Then 1 mashups should be sent to TP
					And mashup 'Account1 mashup1' with accounts 'Account1' and placeholders 'Default' and script 'alert(123)' should be sent to TP
					And 1 mashup should be in profile storage
					And mashup 'mashup1' with placeholders 'Default' and script 'alert(123)' should be in profile storage
			".Execute();
		}

		[Test]
		public void ShouldNotCreateMashupIfThereIsMashupWithSuchName()
		{
			@"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle AddMashupCommand command with args '{""Name"":""mashup1"", ""Placeholders"":""Default"", ""Script"":""alert(123)""}'
				Then 0 mashups should be sent to TP
					And command should return validation error for 'Name' field 'Mashup with the same name already exists'
			".Execute();
		}

		[Test]
		public void ShouldNotCreateMashupIfThereIsMashupWithSuchNameUseWhitespaces()
		{
			@"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle AddMashupCommand command with args '{""Name"":"" mashup1 "", ""Placeholders"":""Default"", ""Script"":""alert(123)""}'
				Then 0 mashups should be sent to TP
					And command should return validation error for 'Name' field 'Mashup with the same name already exists'
			".Execute();
		}

		[Test]
		public void ShouldNotCreateMashupIfNameContainsInvalidChars()
		{
			@"
				Given profile created
				When handle AddMashupCommand command with args '{""Name"":""mashup%^&"", ""Placeholders"":""Default"", ""Script"":""alert(123)""}'
				Then 0 mashups should be sent to TP
					And command should return validation error for 'Name' field 'You can only use letters, numbers, space and underscore symbol in Mashup name'
			".Execute();
		}

		[Test]
		public void ShouldNotCreateMashupIfNameDoesntSpecified()
		{
			@"
				Given profile created
				When handle AddMashupCommand command with args '{""Name"":"""", ""Placeholders"":""Default"", ""Script"":""alert(123)""}'
				Then 0 mashups should be sent to TP
					And command should return validation error for 'Name' field 'Mashup name should be specified'
			".Execute();
		}

		[When("handle AddMashupCommand command with args '$args'")]
		public void HandleAddMashupCommand(string args)
		{
			var addCommand = new AddMashupCommand();
			_response = addCommand.Execute(args);
		}
	}
}