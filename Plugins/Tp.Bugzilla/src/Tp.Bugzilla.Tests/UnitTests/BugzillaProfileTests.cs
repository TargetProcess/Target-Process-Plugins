// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.UnitTests
{
	[TestFixture]
    [Category("PartPlugins0")]
	internal class BugzillaProfileTests
	{
		[Test]
		public void ShouldRemoveWhitespacesFromQueryField()
		{
			var profile = new BugzillaProfile
			              	{
			              		SavedSearches = " query1, query2 "
			              	};
			profile.SavedSearches.Should(Be.EqualTo("query1,query2"), "profile.SavedSearches.Should(Be.EqualTo(\"query1,query2\"))");
		}

		[Test]
		public void ShouldRemoveWhitespacesFromUrlField()
		{
			var profile = new BugzillaProfile
			              	{
			              		Url = " http://url "
			              	};
			profile.Url.Should(Be.EqualTo("http://url"), "profile.Url.Should(Be.EqualTo(\"http://url\"))");
		}
	}
}