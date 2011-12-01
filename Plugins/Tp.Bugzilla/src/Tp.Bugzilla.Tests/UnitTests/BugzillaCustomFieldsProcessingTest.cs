// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Bugzilla.Schemas;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.UnitTests
{
	[TestFixture]
	public class BugzillaCustomFieldsProcessingTest
	{
		[Test]
		public void ShouldProcessSimpleBugzillaCustomField()
		{
			const string cfName = "cf name";
			const string cfValue = "cf value";

			var field = new custom_field {cf_name = cfName, cf_value = cfValue};

			var info = new CustomFieldInfo(field);

			info.Name.Should(Be.EqualTo(cfName));
			info.Values.Should(Be.EquivalentTo(new[] {cfValue}));
		}

		[Test]
		public void ShouldProcessCollectionBugzillaCustomField()
		{
			const string cfName = "cf name";
			const string cfValue1 = "cf value1";
			const string cfValue2 = "cf value2";

			var field = new custom_field { cf_name = cfName, cf_type = "Multiple-Selection Box", cf_values = new cf_values{cf_valueCollection = new cf_valueCollection {cfValue1, cfValue2}}};

			var info = new CustomFieldInfo(field);

			info.Name.Should(Be.EqualTo(cfName));
			info.Values.Should(Be.EquivalentTo(new[] { cfValue1, cfValue2 }));
		}
	}
}