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
    public class WhenOldProfileHasInacceptableName
    {
        [Test]
        public void ShouldReplaceIncorrectCharactersWithUnderscores()
        {
            @"Given account name is 'Account'
					And profile name is 'Profile!@#$%^&*()+={}[]\|/?.>,<`~'
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then 'Profile___________________________converted' plugin profile should be created"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldReplaceIncorrectCharactersWithUnderscoresWhenConvertedTwice()
        {
            @"Given account name is 'Account'
					And profile name is 'Profile!@#$%^&*()+={}[]\|/?.>,<`~'
					And legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				When legacy tfs plugin profile from Target Process converted to new tfs plugin profile
				Then plugin profiles should be created: Profile___________________________converted, Profile___________________________converted_converted"
                .Execute(In.Context<LegacyProfileConverterActionSteps>());
        }
    }
}
