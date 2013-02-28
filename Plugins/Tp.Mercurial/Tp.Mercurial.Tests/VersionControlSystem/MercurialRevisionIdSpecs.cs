// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NUnit.Framework;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Mercurial.Tests.VersionControlSystem
{
	[TestFixture]
	public class MercurialRevisionIdSpecs
	{
		[Test]
		public void ShouldHandlePosixTime()
		{
			var initialTime = DateTime.Today.AddHours(6);
            MercurialRevisionId revisionId = new RevisionId { Time = initialTime, Value = Guid.NewGuid().ToString() };

			RevisionId revisionIdDto = revisionId;
            MercurialRevisionId restoredRevisionId = revisionIdDto;

			restoredRevisionId.Time.Should(Be.EqualTo(initialTime));
		}

		[Test]
		public void ShouldSupportMinTime()
		{
            MercurialRevisionId revisionId = new RevisionId { Time = DateTime.MinValue, Value = Guid.NewGuid().ToString() };
            revisionId.Time.Should(Be.EqualTo(MercurialRevisionId.UtcTimeMin));
		}
	}
}