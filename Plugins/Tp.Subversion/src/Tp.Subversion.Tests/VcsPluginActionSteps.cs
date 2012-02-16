// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NBehave.Narrator.Framework;
using NServiceBus.Saga;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Testing.Common;
using Tp.Integration.Testing.Common.Persisters;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Subversion.Context;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion
{
	[ActionSteps]
	public class VcsPluginActionSteps
	{
		public class TargetProcessRevision
		{
			public string SourceControlID { get; set; }
			public string Description { get; set; }
			public DateTime CommitDate { get; set; }
			public RevisionFile[] RevisionFiles { get; set; }

			public class RevisionFile
			{
				public string FileName { get; set; }
				public FileActionEnum FileAction { get; set; }
			}
		}

		private static VcsPluginContext Context
		{
			get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
		}

		[Given(@"vcs history is:")]
		[When("new revisions committed to vcs:")]
		[Given("vcs commit is: $commit")]
		public void AddToSvnHistory(string commit)
		{
			var revisionInfo = JsonConvert.DeserializeObject<RevisionInfo>(commit, new RevisionIdConverter());
			revisionInfo.Comment = revisionInfo.Comment.Replace("\r", Environment.NewLine);
			Context.Revisions.Add(revisionInfo);
		}

		[Given("culture is set to '$culture'")]
		public void SetCulture(string culture)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
		}

		public class RevisionIdConverter : CustomCreationConverter<RevisionId>
		{
			public override RevisionId Create(Type objectType)
			{
				return null;
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				return new RevisionId {Value = reader.Value.ToString()};
			}
		}

		[Given("vcs history contains $svnUsersCount svn users")]
		public void CreateSvnCommits(int svnUsersCount)
		{
			CreateHistoryWithSvnUsers(svnUsersCount);
		}

		[Given("$svnUsersCount svn users mapped to TP users")]
		public void MapSvnUsersToTpUsers(int svnUsersCount)
		{
			Context.Revisions.Take(svnUsersCount).Select(x => x.Author).ForEach(Context.CreateTpUser);
		}

		[Given("$tpUsersCount unmapped TP users")]
		public void CreateUnmappedUsers(int tpUsersCount)
		{
			for (int i = 0; i < tpUsersCount; i++)
			{
				var name = string.Format("UnmappedUser_{0}", i);
				Context.CreateTpUser(name);
			}
		}

		private static void CreateHistoryWithSvnUsers(int svnUsersCount)
		{
			for (int i = 0; i < svnUsersCount; i++)
			{
				var author = string.Format("Author_{0}", i);
				var revision = new RevisionInfo {Author = author};
				Context.Revisions.Add(revision);
			}
		}

		[Given(@"profile Start Revision is $startRevision")]
		[Given(@"profile Start Revision changed to $startRevision")]
		public void SetStartRevisionFrom(string startRevision)
		{
			Context.Profile.StartRevision = startRevision;
		}

		[Given(@"target process users:")]
		public void CreateTargetProcessUser(string name, string mail)
		{
			Context.CreateTpUser(name, mail);
		}

		[Given(@"target process users with logins, names and mails:")]
		public void CreateTargetProcessUserWithLogin(string login, string name, string mail)
		{
			Context.CreateTpUserWithLogin(login, name, mail);
		}


		[Then(@"revisions created in TP should be:")]
		public void RevisionShouldBeInTp(string commit)
		{
			AssertRevisionCreatedInTpFromCommit(commit);
		}

		private static void AssertRevisionCreatedInTpFromCommit(string commit)
		{
			var revisionEtalon = JsonConvert.DeserializeObject<TargetProcessRevision>(commit);

			var revisionDto = Context.Transport.TpQueue.GetCreatedDtos<RevisionDTO>()
				.SingleOrDefault(x => x.SourceControlID == revisionEtalon.SourceControlID);

			revisionDto.Should(Be.Not.Null, "Revision {0} was not created in TP", commit);

			revisionDto.Description.Should(Be.EqualTo(revisionEtalon.Description));
			revisionDto.CommitDate.Should(Be.EqualTo(revisionEtalon.CommitDate));

			var revisionCreated = Context.Transport.LocalQueue.GetMessages<RevisionCreatedMessage>()
				.Select(x => x.Dto)
				.Single(x => x.SourceControlID == revisionEtalon.SourceControlID);

			var revisionFileDtos = Context.Transport.TpQueue.GetCreatedDtos<RevisionFileDTO>()
				.Where(x => x.RevisionID == revisionCreated.ID)
				.ToList();

			revisionFileDtos.Select(x => x.FileAction).Should(
				Be.EquivalentTo(revisionEtalon.RevisionFiles.Select(x => x.FileAction)));
			revisionFileDtos.Select(x => x.FileName).Should(Be.EquivalentTo(revisionEtalon.RevisionFiles.Select(x => x.FileName)));
		}

		[Given(@"plugin started up")]
		[When("plugin synchronized")]
		public void PluginStartedUp()
		{
			Context.StartPlugin();
		}

		[Given("TP will fail to create revision")]
		public void CreateRevisionFails()
		{
			Context.On.CreateCommand<RevisionDTO>().Reply(x => new TargetProcessExceptionThrownMessage(new Exception()));
		}

		[Then(@"$revisionsCount revision should be created in TP")]
		[Then(@"$revisionsCount revisions should be created in TP")]
		public void RevisionsShouldBeDetected(int revisionsCount)
		{
			Context.Transport.TpQueue.GetCreatedDtos<RevisionDTO>()
				.Count().Should(Be.EqualTo(revisionsCount));
		}

		[AfterScenario]
		[Then("there should be no uncompleted create revision sagas")]
		public void AfterScenario()
		{
			ShouldBeNoUncompletedSagas<CreateRevisionSagaData>();
		}

		[Then("there should be no uncompleted attach to entity sagas")]
		public void ShouldBeNoUncompletedAttachToEntitySagas()
		{
			ShouldBeNoUncompletedSagas<AttachToEntitySagaData>();
		}

		public void ShouldBeNoUncompletedSagas<T>()
		{
			ObjectFactory.GetInstance<TpInMemorySagaPersister>().Get<ISagaEntity>()
				.Where(x => x is T).ToArray()
				.Should(Be.Empty, "There are uncompleted sagas left.");
		}

		[Then("log should contain message: $message")]
		public void LogShouldContain(string message)
		{
			Context.Log.Messages.Contains(message).Should(Be.True, "log does not contain message '{0}'", message);
		}
	}
}