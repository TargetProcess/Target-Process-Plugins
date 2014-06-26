// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
	[TestFixture]
	[ActionSteps]
    [Category("PartPlugins0")]
	public class BugzillaBugInfoStorageSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldRetrieveStoredBugzillaBugInfo()
		{
			@"
				Given bugzilla profile for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And bug 12 has component 'TestComponent'
					And bug 12 has version 'unspecified'
					And bug 12 has platform 'PC'
					And bug 12 has operating system 'Windows'
					And bug 12 has classification 'Unclassified'
					And bug 12 has custom field 'simple custom field' with value '12'
					And bug 12 has custom field 'collection custom field' with collection value: abc, def
					And synchronizing bugzilla bugs
				When retrieving single bug with bugzilla id '12'
				Then bugzilla bug with id 12 retrieved
					And connected bugzilla bug 12 should have component 'TestComponent'
					And connected bugzilla bug 12 should have version 'unspecified'
					And connected bugzilla bug 12 should have platform 'PC'
					And connected bugzilla bug 12 should have operating system 'Windows'
					And connected bugzilla bug 12 should have url 'http://test/com/show_bug.cgi?id=12'
					And connected bugzilla bug 12 should have classification 'Unclassified'
					And connected bugzilla bug 12 should have custom field 'simple custom field' with value '12'
					And connected bugzilla bug 12 should have custom field 'collection custom field' with collection value: abc, def
			"
				.Execute(In.Context<BugSyncActionSteps>()
						.And<BugSyncSpecs>()
						.And<BugzillaBugInfoStorageSpecs>());
		}

		[Test]
		public void ShouldRetrieveMultipleStoredBugzillaBugInfosFromTheSameProfile()
		{
			@"
				Given bugzilla profile for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And bug 12 has component 'TestComponent'
					And bug 12 has version 'unspecified'
					And bug 12 has platform 'PC'
					And bug 12 has operating system 'Windows'
					And bug 12 has classification 'Unclassified'
					And bug 12 has custom field 'simple custom field' with value '12'
					And bug 12 has custom field 'collection custom field' with collection value: abc, def
					And bugzilla contains bug with id 13
					And bug 13 has name 'bug13'
					And bug 13 has component 'TestComponent1'
					And bug 13 has version 'unspecified'
					And bug 13 has platform 'PC'
					And bug 13 has operating system 'Windows'
					And bug 13 has classification 'Unclassified'
					And bug 13 has custom field 'simple custom field' with value '13'
					And bug 13 has custom field 'collection custom field' with collection value: abcf, defe
					And synchronizing bugzilla bugs
				When retrieving multiple bugs with bugzilla ids '12,13'
				Then bugzilla bug with id 12 retrieved
					And bugzilla bug with id 13 retrieved
					And connected bugzilla bug 12 should have url 'http://test/com/show_bug.cgi?id=12'
					And connected bugzilla bug 13 should have url 'http://test/com/show_bug.cgi?id=13'
			"
				.Execute(
					In.Context<BugSyncActionSteps>()
					.And<BugSyncSpecs>()
					.And<BugzillaBugInfoStorageSpecs>()
					.And<BugzillaBugInfoStorageSpecs>());
		}

		[Test]
		public void ShouldRetrieveOnlyBugsFromProfile()
		{
			@"
				Given bugzilla profile 'profile1' for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And synchronizing bugzilla bugs
					And bugzilla profile 'profile2' for project 1 created
					And bugzilla contains bug with id 13
					And bug 13 has name 'bug13'
					And synchronizing bugzilla bugs
					And profile set to 'profile1'
				When retrieving multiple bugs with bugzilla ids '12,13'
				Then bugzilla bug with id 12 retrieved
					And bugzilla bug with id 13 not retrieved
			"
				.Execute(
					In.Context<BugSyncActionSteps>()
					.And<BugSyncSpecs>()
					.And<BugzillaBugInfoStorageSpecs>());
		}

		[Test]
		public void ShouldRetrieveMultipleStoredBugzillaBugInfosFromMultipleProfiles()
		{
			@"
				Given bugzilla profile 'profile1' for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And synchronizing bugzilla bugs
					And bugzilla profile 'profile2' for project 1 created
					And bugzilla contains bug with id 13
					And bug 13 has name 'bug13'
					And synchronizing bugzilla bugs
				When retrieving multiple bugs for multiple profiles with bugzilla ids '12,13'
				Then bugzilla bug with id 12 retrieved
					And bugzilla bug with id 13 retrieved
			"
				.Execute(
					In.Context<BugSyncActionSteps>()
					.And<BugSyncSpecs>()
					.And<BugzillaBugInfoStorageSpecs>());
		}

		[Test]
		public void ShouldUpdateStoredBugzillaBugInfo()
		{
			@"
				Given bugzilla profile for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And bug 12 has platform 'PC'
					And bug 12 has operating system 'Windows'
					And synchronizing bugzilla bugs
					And bug 12 has operating system 'Linux'
					And synchronizing bugzilla bugs
				When retrieving single bug with bugzilla id '12'
				Then bugzilla bug with id 12 retrieved
					And connected bugzilla bug 12 should have platform 'PC'
					And connected bugzilla bug 12 should have operating system 'Linux'
			"
				.Execute(In.Context<BugSyncActionSteps>()
						.And<BugSyncSpecs>()
						.And<BugzillaBugInfoStorageSpecs>());
		}

		[Given("bugzilla profile '$profile' for project $projectId created")]
		public void CreateProfile(string profile, int projectId)
		{
			Context.AddProfile(profile, projectId);
		}

		[Given("bug $bugId has component '$component'")]
		public void SetBugComponent(int bugId, string component)
		{
			Context.BugzillaBugs.SetComponent(bugId, component);
		}

		[Given("bug $bugId has version '$component'")]
		public void SetBugVersion(int bugId, string version)
		{
			Context.BugzillaBugs.SetVersion(bugId, version);
		}

		[Given("bug $bugId has platform '$component'")]
		public void SetBugPlatform(int bugId, string platform)
		{
			Context.BugzillaBugs.SetPlatform(bugId, platform);
		}

		[Given("bug $bugId has operating system '$os'")]
		public void SetBugOs(int bugId, string os)
		{
			Context.BugzillaBugs.SetOperatingSystem(bugId, os);
		}

		[Given("bug $bugId has classification '$classification'")]
		public void SetBugClassification(int bugId, string classification)
		{
			Context.BugzillaBugs.SetClassification(bugId, classification);
		}

		[Given("bug $bugId has custom field '$fieldName' with value '$fieldValue'")]
		public void SetBugCustomField(int bugId, string fieldName, string fieldValue)
		{
			Context.BugzillaBugs.SetCustomField(bugId, fieldName, fieldValue);
		}

		[Given(@"bug $bugId has custom field '$fieldName' with collection value: (?<fieldValue>([^,]+,?\s*)+)")]
		public void SetBugCustomField(int bugId, string fieldName, string[] fieldValue)
		{
			Context.BugzillaBugs.SetCustomField(bugId, fieldName, fieldValue);
		}

		[When("retrieving single bug with bugzilla id '$bugId'")]
		public void RetrieveSingleBugWithBugzillaId(int bugId)
		{
			var bug = GetBugzillaBug(bugId);
			Context.BugzillaBugInfos.Add(bug);
		}

		[When("retrieving multiple bugs with bugzilla ids '$bugIds'")]
		public void RetrieveMultipleBugsWithBugzillaIds(string bugIds)
		{
			var ids = bugIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(Int32.Parse)
				.ToArray();

			var bugs = GetBugzillaBugs(ids);

			Context.BugzillaBugInfos.AddRange(bugs);
		}

		[When("retrieving multiple bugs for multiple profiles with bugzilla ids '$bugIds'")]
		public void RetrieveMultipleBugsForMultipleProfilesWithTheFollowingIds(string bugIds)
		{
			var ids = bugIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(Int32.Parse)
				.ToArray();

			var bugs = GetAllBugzillaBugs(ids);

			Context.BugzillaBugInfos.AddRange(bugs);
		}

		[When("profile set to '$profile'")]
		public void SetProfile(string profile)
		{
			SetProfile(ObjectFactory.GetInstance<IProfileCollectionReadonly>().Single(x => x.Name == profile));
		}

		[Then("bugzilla bug with id $bugzillaBugId not retrieved")]
		public void ShouldCheckTpBugNotConnectedToBugzillaBug(int bugzillaBugId)
		{
			Context.BugzillaBugInfos.Where(x => x.Id == bugzillaBugId.ToString()).Should(Be.Empty);
		}

		[Then("bugzilla bug with id $bugzillaBugId retrieved")]
		public void ShouldCheckTpBugConnectedToBugzillaBug(int bugzillaBugId)
		{
			Context.BugzillaBugInfos.Where(x => x.Id == bugzillaBugId.ToString()).Should(Be.Not.Empty);
		}

		[Then("connected bugzilla bug $bugzillaBugId should have component '$component'")]
		public void ShouldCheckComponent(int bugzillaBugId, string component)
		{
			GetBugzillaBug(bugzillaBugId).Component.Should(Be.EqualTo(component));
		}

		[Then("connected bugzilla bug $bugzillaBugId should have version '$version'")]
		public void ShouldCheckVersion(int bugzillaBugId, string version)
		{
			GetBugzillaBug(bugzillaBugId).Version.Should(Be.EqualTo(version));
		}

		[Then("connected bugzilla bug $bugzillaBugId should have platform '$platform'")]
		public void ShouldCheckPlatform(int bugzillaBugId, string platform)
		{
			GetBugzillaBug(bugzillaBugId).Platform.Should(Be.EqualTo(platform));
		}

		[Then("connected bugzilla bug $bugzillaBugId should have operating system '$os'")]
		public void ShouldCheckOs(int bugzillaBugId, string os)
		{
			GetBugzillaBug(bugzillaBugId).OS.Should(Be.EqualTo(os));
		}

		[Then("connected bugzilla bug $bugzillaBugId should have classification '$classification'")]
		public void ShouldCheckClassification(int bugzillaBugId, string classification)
		{
			GetBugzillaBug(bugzillaBugId).Classification.Should(Be.EqualTo(classification));
		}

		[Then("connected bugzilla bug $bugzillaBugId should have custom field '$fieldName' with value '$fieldValue'")]
		public void ShouldCheckCustomField(int bugzillaBugId, string fieldName, string fieldValue)
		{
			GetBugzillaBug(bugzillaBugId).CustomFields
				.Single(f => f.Name == fieldName)
				.Values.Should(Be.EquivalentTo(new[] {fieldValue}));
		}

		[Then(
			@"connected bugzilla bug $bugzillaBugId should have custom field '$fieldName' with collection value: (?<fieldValue>([^,]+,?\s*)+)"
			)]
		public void ShouldCheckCustomField(int bugzillaBugId, string fieldName, string[] fieldValue)
		{
			GetBugzillaBug(bugzillaBugId).CustomFields
				.Single(f => f.Name == fieldName)
				.Values
				.Should(Be.EquivalentTo(fieldValue));
		}

		[Then("connected bugzilla bug $bugzillaBugId should have url '$url'")]
		public void ShouldCheckUrl(int bugzillaBugId, string url)
		{
			var bug = Context.BugzillaBugInfos
				.Where(x => x.Id == bugzillaBugId.ToString())
				.Single();

			bug.Url.Should(Be.EqualTo(url));
		}

		private BugzillaBugInfo GetBugzillaBug(int bugzillaBugId)
		{
			var tpBugId = Context.StorageRepository
				.GetTargetProcessBugId(bugzillaBugId.ToString())
				.Value;

			return Context.StorageRepository.GetBugzillaBug(tpBugId);
		}

		private IEnumerable<BugzillaBugInfo> GetAllBugzillaBugs(IEnumerable<int> bugzillaBugIds)
		{
			var tpBugIds = new List<int>();
			foreach (var profile in ObjectFactory.GetInstance<IProfileCollectionReadonly>())
			{
				SetProfile(profile);
				tpBugIds.AddRange(bugzillaBugIds.Select(x => Context.StorageRepository.GetTargetProcessBugId(x.ToString())).Where(x => x != null).Select(x => x.Value));
			}
			Context.SetProfile(null);

			return Context.StorageRepository.GetAllBugzillaBugs(tpBugIds);
		}

		public void SetProfile(IProfileReadonly profile)
		{
			Context.SetProfile(profile);
		}

		private IEnumerable<BugzillaBugInfo> GetBugzillaBugs(IEnumerable<int> bugzillaBugIds)
		{
			var tpBugIds = bugzillaBugIds.Select(x => Context.StorageRepository.GetTargetProcessBugId(x.ToString())).Where(x => x != null).Select(x => x.Value);

			return Context.StorageRepository.GetBugzillaBugs(tpBugIds);
		}
	}
}