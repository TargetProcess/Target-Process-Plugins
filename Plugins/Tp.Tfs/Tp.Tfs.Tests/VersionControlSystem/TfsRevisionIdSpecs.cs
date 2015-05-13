// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NUnit.Framework;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;
using Tp.Tfs.VersionControlSystem;

namespace Tp.Tfs.Tests.VersionControlSystem
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class TfsRevisionIdSpecs
	{
		[Test]
		public void ShouldHandlePosixTime()
		{
			var initialTime = DateTime.Today.AddHours(6);
            TfsRevisionId revisionId = new RevisionId { Time = initialTime, Value = Guid.NewGuid().ToString() };

			RevisionId revisionIdDto = revisionId;
            TfsRevisionId restoredRevisionId = revisionIdDto;

			restoredRevisionId.Time.Should(Be.EqualTo(initialTime));
		}

		[Test]
		public void ShouldSupportMinTime()
		{
            TfsRevisionId revisionId = new RevisionId { Time = DateTime.MinValue, Value = Guid.NewGuid().ToString() };
            revisionId.Time.Should(Be.EqualTo(TfsRevisionId.UtcTimeMin));
		}
	}
}