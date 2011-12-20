// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Testing.Common;
using Tp.Plugin.Core.Attachments;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;
using AttachmentPartAddedMessage = Tp.Integration.Messages.EntityLifecycle.Commands.AttachmentPartAddedMessage;

namespace Tp.Bugzilla.Tests.Synchronization
{
	[TestFixture]
	[ActionSteps]
	public class AttachmentSyncSpecs : BugzillaTestBase
	{
		public override void Init()
		{
			base.Init();

			var mockBufferSize = MockRepository.GenerateStub<IBufferSize>();
			mockBufferSize.Stub(x => x.Value).Return(1000);
			ObjectFactory.Configure(x => x.For<IBufferSize>().HybridHttpOrThreadLocalScoped().Use(mockBufferSize));

			ObjectFactory.GetInstance<TransportMock>().On<AddAttachmentPartToMessageCommand>()
				.Reply(x =>
				       	{
				       		if (!x.IsLastPart)
				       		{
				       			return new AttachmentPartAddedMessage {FileName = x.FileName};
				       		}
				       		return new AttachmentCreatedMessage
				       		       	{
				       		       		Dto = new AttachmentDTO
				       		       		      	{
				       		       		      		OriginalFileName = x.FileName,
				       		       		      		GeneralID = x.GeneralId,
				       		       		      		CreateDate = x.CreateDate,
													OwnerID = x.OwnerId,
													Description = x.Description
				       		       		      	}
				       		       	};
				       	});
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(ObjectFactory.GetInstance<PluginDataFolder>().Path, true);
		}

		[Test]
		public void ShouldImportAttachmentsOnBugCreated()
		{
			@"
				Given bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					
					And bug 1 has attachment 'file1' with content 'abcdefj' created on '2010-10-10 13:13'
					And bug 1 has attachment 'file2' with content '123456' created on '2010-10-10 13:13'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 2 attachments
					And bug in TargetProcess with name 'bug1' should have attachment 'file1' with content 'abcdefj' created on '2010-10-10 13:13'
					And bug in TargetProcess with name 'bug1' should have attachment 'file2' with content '123456' created on '2010-10-10 13:13'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AttachmentSyncSpecs>());
		}

		[Test]
		public void ShouldImportAttachmentOnBugCreatedWithDescriptionAndOwner()
		{
			@"
				Given user 'Johnson' with email 'Johnson@mail.com' created in TargetProcess
					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has attachment 'file1'
					And attachment for bug 1 'file1' has owner 'Johnson@mail.com'
					And attachment for bug 1 'file1' has description 'attach description'
					And attachment for bug 1 'file1' has content 'abcdefj'
					And attachment for bug 1 'file1' has creation date '2010-10-10 13:13'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 1 attachments
					And bug in TargetProcess with name 'bug1' should have attachment 'file1' with content 'abcdefj' created on '2010-10-10 13:13'
					And bug in TargetProcess with name 'bug1' should have attachment 'file1' with owner 'Johnson' and description 'attach description'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AttachmentSyncSpecs>());
		}

		[Test]
		public void ShouldNotImportAttachmentsIfThereIsNoAttachments()
		{
			@"
				Given bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 0 attachments
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AttachmentSyncSpecs>());
		}

		[Test]
		public void ShouldAddNewOnlyAttachmentsOnBugUpdated()
		{
			@"
				Given bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has attachment 'file1' with content 'abcdefj' created on '2010-10-10 13:13'
					And bug 1 has attachment 'file2' with content '123456' created on '2010-10-10 13:13'
					And synchronizing bugzilla bugs
					
					And bug 1 has attachment 'file3' with content 'mnbvc' created on '2010-10-11 13:13'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 3 attachments
					And bug in TargetProcess with name 'bug1' should have attachment 'file1' with content 'abcdefj' created on '2010-10-10 13:13'
					And bug in TargetProcess with name 'bug1' should have attachment 'file2' with content '123456' created on '2010-10-10 13:13'
					And bug in TargetProcess with name 'bug1' should have attachment 'file3' with content 'mnbvc' created on '2010-10-11 13:13'
					And no attachments should present on disk
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AttachmentSyncSpecs>());
		}

		[Test]
		public void ShouldRemoveAttachmentsFromDiskIfBugCreationFailed()
		{
			@"
				Given bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has attachment 'file1' with content 'abcdefj' created on '2010-10-10 13:13'
					And TargetProcess is down
				When synchronizing bugzilla bugs
				Then no attachments should present on disk
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AttachmentSyncSpecs>());
		}
		
		[Given("bug $bugId has attachment '$fileName' with content '$content' created on '$creationDate'")]
		public void AddAttachmentToBug(int bugId, string fileName, string content, string creationDate)
		{
			Context.BugzillaBugs.AddAttachment(bugId,
			                                   new attachment
			                                   	{
			                                   		filename = fileName,
			                                   		date = creationDate,
			                                   		data =
			                                   			new data {Value = Convert.ToBase64String(Encoding.ASCII.GetBytes(content))}
			                                   	});
		}

		[Given("attachment for bug $bugId '$fileName' has creation date '$createDate'")]
		public void AddCreateDateToAttachment(int bugId, string fileName, string createDate)
		{
			Context.BugzillaBugs.SetAttachmentTime(bugId, fileName, createDate);
		}

		[Given("attachment for bug $bugId '$fileName' has content '$content'")]
		public void AddContentToAttachment(int bugId, string fileName, string content)
		{
			Context.BugzillaBugs.SetAttachmentContent(bugId, fileName, new data { Value = Convert.ToBase64String(Encoding.ASCII.GetBytes(content)) });
		}

		[Given("bug $bugId has attachment '$fileName'")]
		public void AddAttachmentToBug(int bugId, string fileName)
		{
			Context.BugzillaBugs.AddAttachment(bugId,
											   new attachment
											   {
												   filename = fileName
											   });
		}

		[Given("attachment for bug $bugId '$fileName' has owner '$owner'")]
		public void AddOwnerToAttachment(int bugId, string fileName, string owner)
		{
			Context.BugzillaBugs.SetAttachmentOwner(bugId, fileName, owner);
		}

		[Given("attachment for bug $bugId '$fileName' has description '$description'")]
		public void AddDescriptionToAttachment(int bugId, string fileName, string description)
		{
			Context.BugzillaBugs.SetAttachmentDescription(bugId, fileName, description);
		}

		[Given("TargetProcess is down")]
		public void TargetProcessIsDown()
		{
			TransportMock.ResetAllOnMessageHandlers();
			TransportMock.OnCreateEntityCommand<BugDTO>().Reply(
				x => new TargetProcessExceptionThrownMessage {ExceptionString = "TargetProcess is down."});
		}

		[Then("bug in TargetProcess with name '$bugName' should have $count attachments")]
		public void CheckAttachmentsCount(string bugName, int count)
		{
			TransportMock.TpQueue.GetMessages<AddAttachmentPartToMessageCommand>().Count().Should(Be.EqualTo(count));
		}

		[Then(
			"bug in TargetProcess with name '$bugName' should have attachment '$fileName' with content '$content' created on '$creationDate'"
			)]
		public void CheckAttachment(string bugName, string fileName, string content, string creationDate)
		{
			var part = GetCreatedAttachmentByName(fileName);

			part.CreateDate.Should(Be.EqualTo(CreateDateConverter.ParseFromBugzillaLocalTime(creationDate)));

			Encoding.ASCII.GetString(Convert.FromBase64String(part.BytesSerializedToBase64)).Should(Be.EqualTo(content));
		}

		private AddAttachmentPartToMessageCommand GetCreatedAttachmentByName(string fileName)
		{
			var attachments = new List<AttachmentDTO>();

			foreach (var message in TransportMock.LocalQueue.GetMessages<AttachmentsPushedToTPMessageInternal>())
			{
				attachments.AddRange(message.AttachmentDtos);
			}

			attachments.FirstOrDefault(x => x.OriginalFileName == fileName).Should(Be.Not.Null);

			var part =
				TransportMock.TpQueue.GetMessages<AddAttachmentPartToMessageCommand>().Single(x => x.FileName == fileName);
			return part;
		}

		[Then("bug in TargetProcess with name '$bugName' should have attachment '$fileName' with owner '$ownerLogin' and description '$description'")]
		public void CheckAttachmentDescriptionAndOwner(string bugName, string fileName, string ownerLogin, string description)
		{
			var part = GetCreatedAttachmentByName(fileName);

			part.OwnerId.Should(Be.EqualTo(Context.Users.Single(u => u.Login == ownerLogin).ID));
			part.Description.Should(Be.EqualTo(description));
		}

		[Then("no attachments should present on disk")]
		public void CheckThatThereIsNoAttachments()
		{
			Directory.GetFiles(ObjectFactory.GetInstance<PluginDataFolder>().Path).Count().Should(Be.EqualTo(0));
		}
	}
}
