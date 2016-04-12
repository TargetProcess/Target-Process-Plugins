// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Integration.Messages;
using Tp.MashupManager.CustomCommands;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests.Mashups
{
	[ActionSteps, TestFixture]
    [Category("PartPlugins0")]
	public class GetMashupInfoSpecs : MashupManagerTestBase
	{
		[Test]
		public void ShouldReturnMashupInfoByName()
		{
			@"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle GetMashupInfoCommand command with args '{""Value"":""mashup2""}'
				Then default mashup 'mashup2' should be returned
			"
				.Execute();
		}

		[Test]
		public void ShouldNotReturnMashupInfoByNonExistingName()
		{
			@"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle GetMashupInfoCommand command with args '{""Value"":""mashup3""}'
				Then command should return validation error for 'Name' field 'Mashup with name ""mashup3"" doesn't exist'
			".Execute();
		}

		[When("handle GetMashupInfoCommand command with args '$args'")]
		public void HandleGetMashupInfoCommand(string args)
		{
			var command = new GetMashupInfoCommand();
			_response = command.Execute(args);
		}

		[Then("default mashup '$mashupName' should be returned")]
		public void CheckDefaultMashupReturned(string mashupName)
		{
			var mashup = _response.ResponseData.Deserialize<Mashup>();
			mashup.Name.Should(Be.EqualTo(mashupName), "mashup.Name.Should(Be.EqualTo(mashupName))");
			mashup.Placeholders.ToLower().Should(Be.EqualTo(DefaultPlaceholders.ToLower()), "mashup.Placeholders.ToLower().Should(Be.EqualTo(DefaultPlaceholders.ToLower()))");
			mashup.Files.First().Content.Should(Be.EqualTo(DefaultScript), "mashup.Files.First().Content.Should(Be.EqualTo(DefaultScript))");
		}
	}
}