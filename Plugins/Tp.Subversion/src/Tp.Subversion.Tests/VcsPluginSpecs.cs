// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Subversion
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class VcsPluginSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (SubversionPluginProfile).Assembly,
				                                                                        new List<Assembly> {typeof (Command).Assembly})));
		}

		[Test]
		public void ShouldScanForRevisionsUponStartup()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#123 Comment 1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
				When plugin started up
				Then 1 revision should be created in TP
					And revisions created in TP should be:
					|commit|
					|{SourceControlID:1, Description:""#123 Comment 1"", CommitDate:""10.01.2000"", RevisionFiles:[{FileName:""/root/file1.txt"", FileAction:""Add""}]}|"
				.Execute(In.Context<VcsPluginActionSteps>());
		}

		[Test]
		public void ShouldScanRevisionsWithEntityIdOnly()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""Comment 1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					|{Id:2, Comment:""#123 Comment 2"", Author:""Smith"",Time:""10.02.2000"", Entries:[{Path:""/root/file2.txt"", Action:""Modify""}]}|
				When plugin started up
				Then 1 revision should be created in TP
					And revisions created in TP should be:
					|commit|
					|{SourceControlID:2, Description:""#123 Comment 2"", CommitDate:""10.02.2000"", RevisionFiles:[{FileName:""/root/file2.txt"", FileAction:""Modify""}]}|"
				.Execute(In.Context<VcsPluginActionSteps>());
		}

		[Test]
		public void ShouldDetectNewRevisionsWithEntityIdOnly()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#123 Comment 1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					|{Id:2, Comment:""#123 Comment 2"", Author:""Smith"",Time:""10.02.2000"", Entries:[{Path:""/root/file2.txt"", Action:""Modify""}]}|
					And plugin started up
				When new revisions committed to vcs:
					|commit|
					|{Id:3, Comment:""#123 Comment 3"", Author:""Tomara"",Time:""10.03.2000"", Entries:[{Path:""/root/file3.txt"", Action:""Delete""}]}|
					And plugin synchronized
				Then 1 revision should be created in TP
					And revisions created in TP should be:
					|commit|
					|{SourceControlID:3, Description:""#123 Comment 3"", CommitDate:""10.03.2000"", RevisionFiles:[{FileName:""/root/file3.txt"", FileAction:""Delete""}]}|
				"
				.Execute(In.Context<VcsPluginActionSteps>());
		}

		[Test]
		public void ShouldImportRevisionsFromSpecifiedInProfile()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#123 Comment 1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					|{Id:2, Comment:""#123 Comment 2"", Author:""Smith"",Time:""10.02.2000"", Entries:[{Path:""/root/file2.txt"", Action:""Modify""}]}|
					|{Id:3, Comment:""#123 Comment 3"", Author:""Tomara"",Time:""10.03.2000"", Entries:[{Path:""/root/file3.txt"", Action:""Delete""}]}|
					And profile Start Revision is 3
				When plugin started up
				Then 1 revision should be created in TP
					And revisions created in TP should be:
					|commit|
					|{SourceControlID:3, Description:""#123 Comment 3"", CommitDate:""10.03.2000"", RevisionFiles:[{FileName:""/root/file3.txt"", FileAction:""Delete""}]}|
				"
				.Execute(In.Context<VcsPluginActionSteps>());
		}

		[Test]
		public void ShouldHandleCreateRevisionFailure()
		{
			@"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#123 Comment 1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And TP will fail to create revision
				When plugin started up
				Then there should be no uncompleted create revision sagas"
				.Execute(In.Context<VcsPluginActionSteps>());
		}
	}
}