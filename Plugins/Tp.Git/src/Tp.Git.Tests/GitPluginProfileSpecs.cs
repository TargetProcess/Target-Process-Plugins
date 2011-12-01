// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NUnit.Framework;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests
{
	[TestFixture]
	public class GitPluginProfileSpecs
	{
		private GitPluginProfile _profile;
		private PluginProfileErrorCollection _errors;

		[SetUp]
		public void Init()
		{
			_profile = new GitPluginProfile();
			_errors = new PluginProfileErrorCollection();
		}
		[Test]
		public void ShouldValidateStartRevisionShouldBeNotBeforeMin()
		{
			_profile.StartRevision = GitRevisionId.UtcTimeMin.AddDays(-1).ToShortDateString();
			_profile.Validate(_errors);

			var startRevisionError = _errors.Single();
			startRevisionError.FieldName.Should(Be.EqualTo("StartRevision"));
			startRevisionError.Message.Should(Be.EqualTo("Start Revision Date should be not before 1/1/1970"));
		}
		[Test]
		public void ShouldValidateStartRevisionShouldBeNotBehindMax()
		{
			_profile.StartRevision = GitRevisionId.UtcTimeMax.AddDays(1).ToShortDateString();
			_profile.Validate(_errors);

			var startRevisionError = _errors.Single();
			startRevisionError.FieldName.Should(Be.EqualTo("StartRevision"));
			startRevisionError.Message.Should(Be.EqualTo("Start Revision Date should be not behind 1/19/2038"));
		}

		[Test]
		public void ShouldHandleInvalidStartRevision()
		{
			_profile.StartRevision = "that's not revision at all :(";
			_profile.Validate(_errors);

			var startRevisionError = _errors.Single();
			startRevisionError.FieldName.Should(Be.EqualTo("StartRevision"));
			startRevisionError.Message.Should(Be.EqualTo("Start Revision Date should be specified in mm/dd/yyyy format"));
		}
	}
}