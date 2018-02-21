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
    public class WhenOldProfileHasInacceptableName
    {
        [Test]
        public void ShouldReplaceIncorrectCharactersWithUnderscores()
        {
            @"Given account name is 'Account'
					And profile name is 'Profile!@#$%^&*()+={}[]\|/?.>,<`~'
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then 'Profile__________________________' plugin profile should be created"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldReplaceIncorrectCharactersWithUnderscoresWhenConvertedTwice()
        {
            @"Given account name is 'Account'
					And profile name is 'Profile!@#$%^&*()+={}[]\|/?.>,<`~'
					And legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				When legacy subversion plugin profile from Target Process converted to new subversion plugin profile
				Then plugin profiles should be created: Profile__________________________, Profile__________________________ _re-converted_"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }
    }
}
