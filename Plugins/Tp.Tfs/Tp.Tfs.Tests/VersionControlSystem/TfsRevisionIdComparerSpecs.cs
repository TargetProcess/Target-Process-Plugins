// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using NUnit.Framework;
using Tp.Tfs.VersionControlSystem;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Tfs.Tests.VersionControlSystem
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class TfsRevisionIdComparerSpecs
	{
		private TfsRevisionIdComparer _comparer;
		private RevisionRange[] _revisionRanges;

		[SetUp]
		public void Init()
		{
            _comparer = new TfsRevisionIdComparer();
			_revisionRanges = new[]
			                  	{
			                  		CreateRevisionRange(1, 5),
			                  		CreateRevisionRange(3, 7),
			                  		CreateRevisionRange(5, 10),
			                  		CreateRevisionRange(1, 20),
			                  		CreateRevisionRange(1, 15)
			                  	};
		}

		[Test]
		public void ShouldFindMinFromRevision()
		{
            ((TfsRevisionId)_comparer.FindMinFromRevision(_revisionRanges)).Time.Should(
                Be.EqualTo(((TfsRevisionId)_revisionRanges[3].FromChangeset).Time), "((TfsRevisionId)_comparer.FindMinFromRevision(_revisionRanges)).Time.Should( Be.EqualTo(((TfsRevisionId)_revisionRanges[3].FromChangeset).Time))");
		}

		[Test]
		public void ShouldFindMaxToRevision()
		{
            ((TfsRevisionId)_comparer.FindMaxToRevision(_revisionRanges)).Time.Should(
                Be.EqualTo(((TfsRevisionId)_revisionRanges[2].ToChangeset).Time), "((TfsRevisionId)_comparer.FindMaxToRevision(_revisionRanges)).Time.Should( Be.EqualTo(((TfsRevisionId)_revisionRanges[2].ToChangeset).Time))");
		}

		[Test]
		public void ShouldDetectIfOneRevisionBeforeRange()
		{
			_comparer.Is(_revisionRanges[4].FromChangeset).Before(_revisionRanges[2]).Should(Be.True, "_comparer.Is(_revisionRanges[4].FromChangeset).Before(_revisionRanges[2]).Should(Be.True)");
		}

		[Test]
		public void ShouldDetectIfOneRevisionBehindRange()
		{
			_comparer.Is(_revisionRanges[4].ToChangeset).Behind(_revisionRanges[0]).Should(Be.True, "_comparer.Is(_revisionRanges[4].ToChangeset).Behind(_revisionRanges[0]).Should(Be.True)");
		}

		[Test]
		public void ShouldDetectIfOneRevisionBelongRange()
		{
			_comparer.Is(_revisionRanges[4].FromChangeset).Belong(_revisionRanges[3]).Should(Be.True, "_comparer.Is(_revisionRanges[4].FromChangeset).Belong(_revisionRanges[3]).Should(Be.True)");
		}

		[Test]
		public void ShouldDetectIfOneRevisionGreaterThanOther()
		{
			_comparer.Is(_revisionRanges[0].ToChangeset).GreaterThan(_revisionRanges[0].FromChangeset).Should(Be.True, "_comparer.Is(_revisionRanges[0].ToChangeset).GreaterThan(_revisionRanges[0].FromChangeset).Should(Be.True)");
		}

		[Test]
		public void ShouldDetectIfOneRevisionLessThanOther()
		{
			_comparer.Is(_revisionRanges[0].FromChangeset).LessThan(_revisionRanges[0].ToChangeset).Should(Be.True, "_comparer.Is(_revisionRanges[0].FromChangeset).LessThan(_revisionRanges[0].ToChangeset).Should(Be.True)");
		}

		#region Helpers

		private static DateTime Date(string date)
		{
			return DateTime.Parse(date, CultureInfo.InvariantCulture.DateTimeFormat);
		}

		private static RevisionRange CreateRevisionRange(Int32 from, Int32 to)
		{
            return new RevisionRange(new TfsRevisionId { Value = from.ToString() }, new TfsRevisionId { Value = to.ToString() });
		}

		#endregion
	}
}