// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Comments.DSL;
using Tp.SourceControl.Messages;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.Comments
{
    [ActionSteps]
    public class CommentParserActionSteps
    {
        private IEnumerable<IAction> _parsedActions;
        private string _comment;

        [BeforeScenario]
        public void BeforeScenario()
        {
            ObjectFactory.Initialize(x => x.For<IActionFactory>().Use<ActionFactory>());
        }

        [Given("comment: $comment")]
        public void GivenComment(string comment)
        {
            _comment = comment;
        }

        [When("parsed")]
        public void WhenParsed()
        {
            _parsedActions = new CommentParser().Parse(new RevisionDTO { Description = _comment }, 0);
        }

        [Then("attach to entity $id message should be created")]
        public void EntityIdFoundMessageShouldBeCreated(int id)
        {
            _parsedActions.OfType<AssignRevisionToEntityAction>()
                .SingleOrDefault(x => x.EntityId == id)
                .Should(Be.Not.Null,
                    "_parsedActions.OfType<AssignRevisionToEntityAction>().SingleOrDefault(x => x.EntityId == id).Should(Be.Not.Null)");
        }
    }
}
