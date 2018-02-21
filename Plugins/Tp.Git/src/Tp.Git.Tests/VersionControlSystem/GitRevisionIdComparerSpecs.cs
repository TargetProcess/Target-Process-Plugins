﻿// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using NUnit.Framework;
using Tp.Git.VersionControlSystem;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests.VersionControlSystem
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class GitRevisionIdComparerSpecs
    {
        private GitRevisionIdComparer _comparer;
        private RevisionRange[] _revisionRanges;

        [SetUp]
        public void Init()
        {
            _comparer = new GitRevisionIdComparer();
            _revisionRanges = new[]
            {
                CreateRevisionRange(Date("1.10.2001"), Date("2.10.2001")),
                CreateRevisionRange(Date("1.15.2001"), Date("4.12.2001")),
                CreateRevisionRange(Date("1.16.2001"), Date("7.11.2001")),
                CreateRevisionRange(Date("1.01.2001"), Date("3.10.2001")),
                CreateRevisionRange(Date("1.13.2001"), Date("6.14.2001"))
            };
        }

        [Test]
        public void ShouldFindMinFromRevision()
        {
            ((GitRevisionId) _comparer.FindMinFromRevision(_revisionRanges)).Time.Should(
                Be.EqualTo(((GitRevisionId) _revisionRanges[3].FromChangeset).Time),
                "((GitRevisionId) _comparer.FindMinFromRevision(_revisionRanges)).Time.Should(Be.EqualTo(((GitRevisionId) _revisionRanges[3].FromChangeset).Time))");
        }

        [Test]
        public void ShouldFindMaxToRevision()
        {
            ((GitRevisionId) _comparer.FindMaxToRevision(_revisionRanges)).Time.Should(
                Be.EqualTo(((GitRevisionId) _revisionRanges[2].ToChangeset).Time),
                "((GitRevisionId) _comparer.FindMaxToRevision(_revisionRanges)).Time.Should(Be.EqualTo(((GitRevisionId) _revisionRanges[2].ToChangeset).Time))");
        }

        [Test]
        public void ShouldDetectIfOneRevisionBeforeRange()
        {
            _comparer.Is(_revisionRanges[4].FromChangeset)
                .Before(_revisionRanges[2])
                .Should(Be.True, "_comparer.Is(_revisionRanges[4].FromChangeset).Before(_revisionRanges[2]).Should(Be.True)");
        }

        [Test]
        public void ShouldDetectIfOneRevisionBehindRange()
        {
            _comparer.Is(_revisionRanges[4].ToChangeset)
                .Behind(_revisionRanges[0])
                .Should(Be.True, "_comparer.Is(_revisionRanges[4].ToChangeset).Behind(_revisionRanges[0]).Should(Be.True)");
        }

        [Test]
        public void ShouldDetectIfOneRevisionBelongRange()
        {
            _comparer.Is(_revisionRanges[4].FromChangeset)
                .Belong(_revisionRanges[3])
                .Should(Be.True, "_comparer.Is(_revisionRanges[4].FromChangeset).Belong(_revisionRanges[3]).Should(Be.True)");
        }

        [Test]
        public void ShouldDetectIfOneRevisionGreaterThanOther()
        {
            _comparer.Is(_revisionRanges[0].ToChangeset)
                .GreaterThan(_revisionRanges[0].FromChangeset)
                .Should(Be.True,
                    "_comparer.Is(_revisionRanges[0].ToChangeset).GreaterThan(_revisionRanges[0].FromChangeset).Should(Be.True)");
        }

        [Test]
        public void ShouldDetectIfOneRevisionLessThanOther()
        {
            _comparer.Is(_revisionRanges[0].FromChangeset)
                .LessThan(_revisionRanges[0].ToChangeset)
                .Should(Be.True, "_comparer.Is(_revisionRanges[0].FromChangeset).LessThan(_revisionRanges[0].ToChangeset).Should(Be.True)");
        }

        #region Helpers

        private static DateTime Date(string date)
        {
            return DateTime.Parse(date, CultureInfo.InvariantCulture.DateTimeFormat);
        }

        private static RevisionRange CreateRevisionRange(DateTime from, DateTime to)
        {
            return new RevisionRange(new GitRevisionId { Time = from }, new GitRevisionId { Time = to });
        }

        #endregion
    }
}
