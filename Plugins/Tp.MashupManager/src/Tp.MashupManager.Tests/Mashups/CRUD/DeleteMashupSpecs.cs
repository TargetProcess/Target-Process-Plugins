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
    [Category("PartPlugins1")]
    public class DeleteMashupSpecs : MashupManagerTestBase
    {
        [Test]
        public void ShouldDeleteMashupByName()
        {
            @"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle DeleteMashupCommand command with args '{""Name"":""mashup1"", ""Placeholders"":"""", ""Files"":[]}'
				Then mashup 'mashup1' should be cleared in TP and profile
					And profile should have following mashups: mashup2
			".Execute();
        }

        [Test]
        public void ShouldDeleteMashupByNameForSpecificAccount()
        {
            @"
				Given profile created for account 'Account1'
					And profile mashups are: mashup1, mashup2
				When handle DeleteMashupCommand command with args '{""Name"":""mashup1"", ""Placeholders"":"""", ""Files"":[]}'
				Then mashup 'mashup1' should be cleared in profile
					And mashup 'mashup1' should be cleared in TP
					And profile should have following mashups: mashup2
			".Execute();
        }

        [Test]
        public void ShouldTrimMashupNameBeforeDelete()
        {
            @"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle DeleteMashupCommand command with args '{""Name"":""  mashup1  "", ""Placeholders"":"""", ""Files"":[]}'
				Then mashup 'mashup1' should be cleared in TP and profile
					And profile should have following mashups: mashup2
			".Execute();
        }

        [When("handle DeleteMashupCommand command with args '$args'")]
        public void HandleDeleteMashupCommand(string args)
        {
            var command = new DeleteMashupCommand();
            command.Execute(args);
        }
    }
}
