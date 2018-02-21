// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion.LegacyProfileConversionFeature
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class WhenThereIsAnOldPluginProfile
    {
        [Test]
        public void ShouldConvertAllSubversionPluginProfilesToNewPlugin()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And user 'tpuser1' created
					And user 'tpuser2' created
					And subversion repository is 'http://host/subversionrepo'
					And sync interval is 15
					And subversion login is 'login'
					And subversion password is 'password'
					And subversion starting revision is 25
					And subversion revision 25 is imported
					And user mapping is:
					|subversion|targetprocess|
					|svnuser1|tpuser1|
					|svnuser2|tpuser2|
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then plugin 'Subversion' should have account 'Account'
					And 'ProfileName' plugin profile should be created
					And subversion repository should be 'http://host/subversionrepo'
					And sync interval should be predefined as 5
					And subversion login should be 'login'
					And subversion password should be 'password'
					And subversion starting revision should be 25
					And user mapping should be:
					|subversion|targetprocess|
					|svnuser1|tpuser1|
					|svnuser2|tpuser2|
					And profile should have tp users: tpuser1, tpuser2
					And profile should have revisions: 25
					And profile should be initialized"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldMigrateRevisionsToExistingConvertedProfile()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And user 'tpuser1' created
					And subversion repository is 'http://host/subversionrepo'
					And sync interval is 15
					And subversion login is 'login'
					And subversion password is 'password'
					And subversion starting revision is 25
					And user mapping is:
					|subversion|targetprocess|
					|svnuser1|tpuser1|
					|svnuser2|tpuser2|
					And legacy subversion plugin profile from Target Process converted to new subversion plugin profile
					And subversion revision 25 is imported
				When revisions migrated from old profile to the new one
				Then profile should have revisions: 25
					And profile should be initialized"
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
					|subversion|targetprocess|
					|svnuser1|tpuser1@company.com|
					|svnuser2|tpuser2@company.com|
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then user mapping should be:
					|subversion|targetprocess|
					|svnuser1|tpuser1|
					|svnuser2|tpuser2|"
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
						|subversion|targetprocess|
						|svnuser1|tpuser1@company.com|
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then user mapping should be:
					|subversion|targetprocess|
					|svnuser1|tpuser1_active|"
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
					|subversion|targetprocess|
					|svnuser1|tpuser1|
					|svnuser2|tpuser2|
					|svnuser3|tpuser3|
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then user mapping should be:
					|subversion|targetprocess|
					|svnuser1|tpuser1|
					|svnuser3|tpuser3|"
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
					|subversion|targetprocess|
					|svnuser1|requester|
					|svnuser2|tpuser|
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then user mapping should be:
					|subversion|targetprocess|
					|svnuser2|tpuser|
					And 1 user snould be mapped"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldAddProfilesWhenConverterRunTheSecondTime()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
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
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then subversion starting revision should be 236"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }
    }
}
