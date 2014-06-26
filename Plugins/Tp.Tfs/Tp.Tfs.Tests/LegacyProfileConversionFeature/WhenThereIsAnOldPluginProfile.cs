// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.Tfs.Tests.LegacyProfileConversionFeature
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class WhenThereIsAnOldPluginProfile
	{
        [Test]
        public void ShouldMigrateRevisionsToExistingConvertedProfile()
        {
            string.Format(@"Given account name is 'Account'
					And profile name is 'ProfileName'
					And user 'tpuser1' created
					And tfs server uri is '{0}'
                    And team project name is '{1}'
					And sync interval is 15
					And tfs login is '{2}'
					And tfs password is '{3}'
					And tfs starting revision is 25
					And user mapping is:
					|tfs|targetprocess|
					|tfsuser1|tpuser1|
					|tfsuser2|tpuser2|
					And legacy tfs plugin profile from Target Process converted to new tfs plugin profile
					And tfs revision 25 is imported
				When revisions migrated from old profile to the new one
				Then profile should have revisions: 25
					And profile should be initialized", 
                    ConfigHelper.Instance.TestCollection,
                    ConfigHelper.Instance.TestCollectionProject,
                    ConfigHelper.Instance.Login,
                    ConfigHelper.Instance.Password)
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldSupportTpUserSpecifiedByEmail()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And tp user with login 'tpuser1' and email 'tpuser1@company.com'
					And tp user with login 'tpuser2' and email 'tpuser2@company.com'
					And user mapping is:
					|tfs|targetprocess|
					|tfsuser1|tpuser1@company.com|
					|tfsuser2|tpuser2@company.com|
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then user mapping should be:
					|tfs|targetprocess|
					|tfsuser1|tpuser1|
					|tfsuser2|tpuser2|"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldSkipDeletedUsers()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And deleted tp user with login 'tp user1' and email 'tpuser0@company.com' 
					And tp user with login 'tpuser1_active' and email 'tpuser1@company.com'
					And user mapping is:
						|tfs|targetprocess|
						|tfsuser1|tpuser1@company.com|
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then user mapping should be:
					|tfs|targetprocess|
					|tfsuser1|tpuser1_active|"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldSkipUsersThatDoNotExistInTp()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And user 'tpuser1' created
					And user 'tpuser3' created
					And user mapping is:
					|tfs|targetprocess|
					|tfsuser1|tpuser1|
					|tfsuser2|tpuser2|
					|tfsuser3|tpuser3|
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then user mapping should be:
					|tfs|targetprocess|
					|tfsuser1|tpuser1|
					|tfsuser3|tpuser3|"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldSkipRequesters()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And requester 'requester' created
					And user 'tpuser' created
					And user mapping is:
					|tfs|targetprocess|
					|tfsuser1|requester|
					|tfsuser2|tpuser|
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then user mapping should be:
					|tfs|targetprocess|
					|tfsuser2|tpuser|
					And 1 user snould be mapped"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldAddProfilesWhenConverterRunTheSecondTime()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then 2 plugin profiles should be created
					And plugin profile names should be unique"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldImportRevisionsStartingFromOldOnes()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And the last imported revision is 235
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then tfs starting revision should be 236"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }
	}
}
