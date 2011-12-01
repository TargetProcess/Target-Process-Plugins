// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Comments.DSL;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.Comments
{
	[TestFixture, Ignore]
	public class CommentParserSpecs
	{
		[Test]
		public void NullCommentShouldBeParsed()
		{
			@"
				When parsed null comment
				Then 0 commands should be parsed"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void EmptyCommentShouldBeParsed()
		{
			@"
				When parsed comment:  
				Then 0 commands should be parsed"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void GeneralCommandShouldBeParsed()
		{
			@"
				When parsed comment: id:123 time:1 state:fixed but not verified comment:hello world  
				Then 1 command should be parsed
					And commands should be:
					|ids	|actions								|times								|comments			|
					|123	|fixed but not verified	|spent:1,left:-1			|hello world	|"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void MultiplyCommandsShouldBeParsed()
		{
			@"
				When parsed comment: id:123 id:456 time:1 state:fixed comment:hello world  time:2 state:fixed2 comment:comment2
				Then 1 command should be parsed
					And commands should be:
					|ids			|actions			|times														|comments							|
					|123,456	|fixed,fixed2	|spent:1,left:-1,spent:2,left:-1	|hello world,comment2	|"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void TimeLeftCommandsShouldBeParsed()
		{
			@"
				When parsed comment: id:123 id:456 time:1:2 state:fixed comment:hello world  time:2:3 state:fixed2 comment:comment2
				Then 1 command should be parsed
					And commands should be:
					|ids			|actions			|times													|comments							|
					|123,456	|fixed,fixed2	|spent:1,left:2,spent:2,left:3	|hello world,comment2	|"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void IdOnlyShouldBeParsed()
		{
			@"Given comment is: id: 1
				When comment parsed
				Then 1 command should be parsed
					And commands should be:
					|ids			|actions	|times	|comments	|
					|1				|					|				|					|"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void SeveralIdsShouldBeParsed()
		{
			@"Given comment: US#267 implemented and Bug#272 fixed
				When parsed
				Then attach to entity 267 message should be created
					and attach to entity 272 message should be created"
				.Execute(In.Context<CommentParserActionSteps>());
		}

		[Test]
		public void ShouldSupport()
		{
			ObjectFactory.Configure(x => x.For<IActivityLogger>().Use<LogMock>());
			ObjectFactory.Configure(x => x.For<IActionFactory>().Use<ActionFactory>());
			var comment = string.Format("added headerRenderer to WindowShade control{0} added fix to allow CanvasButton to work within a Repeater", Environment.NewLine);
			var info = new RevisionInfo {Comment = comment};
			new CommentParser().ParseAssignToEntityAction(info).Should(Be.Empty);
		}
	}
}