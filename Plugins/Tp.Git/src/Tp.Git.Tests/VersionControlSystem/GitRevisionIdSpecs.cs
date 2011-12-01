// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tp.Git.VersionControlSystem;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests.VersionControlSystem
{
	[TestFixture]
	public class GitRevisionIdSpecs
	{
		[Test]
		public void ShouldHandlePosixTime()
		{
			var initialTime = DateTime.Today.AddHours(6);
			GitRevisionId revisionId = initialTime;

			RevisionId revisionIdDto = revisionId;
			GitRevisionId restoredRevisionId = revisionIdDto;

			restoredRevisionId.Value.Should(Be.EqualTo(initialTime));
		}

		[Test]
		public void ShouldSupportMinTime()
		{
			GitRevisionId revisionId = DateTime.MinValue;
			revisionId.Value.Should(Be.EqualTo(GitRevisionId.UtcTimeMin));
		}
	}
}