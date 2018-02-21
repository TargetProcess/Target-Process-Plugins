// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus.Serialization;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Comments.Actions;
using Tp.SourceControl.Messages;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.SerializationPatches
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class PatchTests
    {
        [SetUp]
        public void Init()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<SubversionRegistry>());
            ObjectFactory.Configure(
                x =>
                    x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly,
                        new List<Assembly>
                            { typeof(Command).Assembly })));
        }

        [Test]
        public void NewRevisionRangeDetectedLocalMessagePatch()
        {
            const string oldMessage =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<object name="""" type=""TK0"" assembly="""">
	<!-- Data section : Don't edit any attributes ! -->
	<items>
		<item name=""0"" type=""TK1"" assembly="""">
			<properties>
				<property name=""Range"" type=""TK2"" assembly="""">
					<properties>
						<property name=""FromChangeset"" type=""TK3"" assembly="""">
							<properties>
								<property name=""Value"" type=""TK4"" assembly="""">1</property>
							</properties>
						</property>
						<property name=""ToChangeset"" type=""TK3"" assembly="""">
							<properties>
								<property name=""Value"" type=""TK4"" assembly="""">2</property>
							</properties>
						</property>
					</properties>
				</property>
			</properties>
		</item>
	</items>
	<!-- TypeDictionary : Don't edit anything in this section at all ! -->
	<typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib"">
		<items>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK4</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Int64</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.VersionControlSystem.RevisionRange</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK3</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Subversion.SvnRevisionId</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages.PluginLifecycle.IPluginLocalMessage[]</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Workflow.NewRevisionRangeDetectedLocalMessage</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
		</items>
	</typedictionary>
</object>";

            var newRevisionRangeDetectedLocalMessage = Deserialize(oldMessage);
            ((NewRevisionRangeDetectedLocalMessage) newRevisionRangeDetectedLocalMessage[0]).Range.FromChangeset.Value.Should(
                Be.EqualTo("1"),
                "((NewRevisionRangeDetectedLocalMessage) newRevisionRangeDetectedLocalMessage[0]).Range.FromChangeset.Value.Should(Be.EqualTo(\"1\"))");
            ((NewRevisionRangeDetectedLocalMessage) newRevisionRangeDetectedLocalMessage[0]).Range.ToChangeset.Value.Should(
                Be.EqualTo("2"),
                "((NewRevisionRangeDetectedLocalMessage) newRevisionRangeDetectedLocalMessage[0]).Range.ToChangeset.Value.Should(Be.EqualTo(\"2\"))");
        }

        [Test]
        public void RevisionRangeInProfileStorage()
        {
            const string oldXml =
                @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>Tp.Subversion.VersionControlSystem.RevisionRange, Tp.Subversion</Type>
  <Version>2</Version>
  <string>{""__type"":""RevisionRange:#Tp.Subversion.VersionControlSystem"",""FromChangeset"":{""__type"":""SvnRevisionId:#Tp.Subversion.Subversion"",""_value"":1},""ToChangeset"":{""__type"":""SvnRevisionId:#Tp.Subversion.Subversion"",""_value"":2}}</string>
</Value>
";
            var result =
                BlobSerializer.Deserialize(XDocument.Parse(oldXml), new TypeNameWithoutVersion(typeof(RevisionRange)).Value) as
                    RevisionRange;
            result.FromChangeset.Value.Should(Be.EqualTo("1"), "result.FromChangeset.Value.Should(Be.EqualTo(\"1\"))");
            result.ToChangeset.Value.Should(Be.EqualTo("2"), "result.ToChangeset.Value.Should(Be.EqualTo(\"2\"))");
        }

        [Test]
        public void NewRevisionDetectedLocalMessagePatch()
        {
            const string oldMessage =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<object name="""" type=""TK0"" assembly="""">
	<!-- Data section : Don't edit any attributes ! -->
	<items>
		<item name=""0"" type=""TK1"" assembly="""">
			<properties>
				<property name=""Revision"" type=""TK2"" assembly="""">
					<properties>
						<property name=""Id"" type=""TK3"" assembly="""">
							<properties>
								<property name=""Value"" type=""TK4"" assembly="""">1</property>
							</properties>
						</property>
						<property name=""Comment"" type=""TK5"" assembly="""">comment</property>
						<property name=""Author"" type=""TK5"" assembly="""">author</property>
						<property name=""Time"" type=""TK6"" assembly="""">12/08/2011 16:31:31</property>
						<property name=""Entries"" type=""TK7"" assembly="""">
							<items>
								<item name=""0"" type=""TK8"" assembly="""">
									<properties>
										<property name=""Path"" type=""TK5"" assembly="""">path</property>
										<property name=""Action"" type=""TK9"" assembly="""">Add</property>
									</properties>
								</item>
							</items>
						</property>
					</properties>
				</property>
				<property name=""SagaId"" type=""TK10"" assembly="""">00000000-0000-0000-0000-000000000000</property>
			</properties>
		</item>
	</items>
	<!-- TypeDictionary : Don't edit anything in this section at all ! -->
	<typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib"">
		<items>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK10</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Guid</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Messages.NewRevisionDetectedLocalMessage</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK4</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Int64</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK8</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.VersionControlSystem.RevisionEntryInfo</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK9</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Common.FileActionEnum</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK6</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.DateTime</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK7</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.VersionControlSystem.RevisionEntryInfo[]</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages.PluginLifecycle.IPluginLocalMessage[]</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK5</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.String</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.VersionControlSystem.RevisionInfo</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK3</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Subversion.SvnRevisionId</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
		</items>
	</typedictionary>
</object>";

            var newRevisionDetectedLocalMessage = Deserialize(oldMessage);
            var revisionInfo = ((NewRevisionDetectedLocalMessage) newRevisionDetectedLocalMessage[0]).Revision;
            revisionInfo.Author.Should(Be.EqualTo("author"), "revisionInfo.Author.Should(Be.EqualTo(\"author\"))");
            revisionInfo.Comment.Should(Be.EqualTo("comment"), "revisionInfo.Comment.Should(Be.EqualTo(\"comment\"))");
            revisionInfo.Id.Value.Should(Be.EqualTo("1"), "revisionInfo.Id.Value.Should(Be.EqualTo(\"1\"))");
            revisionInfo.Entries[0].Action.Should(Be.EqualTo(FileActionEnum.Add),
                "revisionInfo.Entries[0].Action.Should(Be.EqualTo(FileActionEnum.Add))");
            revisionInfo.Entries[0].Path.Should(Be.EqualTo("path"), "revisionInfo.Entries[0].Path.Should(Be.EqualTo(\"path\"))");
        }

        private static object[] Deserialize(string oldMessage)
        {
            var byteArray = Encoding.ASCII.GetBytes(oldMessage);
            var stream = new MemoryStream(byteArray);

            var newRevisionRangeDetectedLocalMessage = new AdvancedXmlSerializer().Deserialize(stream) as object[];
            return newRevisionRangeDetectedLocalMessage;
        }

        [Test]
        public void RevisionCreateLocalMessagePatch()
        {
            const string oldMessage =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<object name="""" type=""TK0"" assembly="""">
	<!-- Data section : Don't edit any attributes ! -->
	<items>
		<item name=""0"" type=""TK1"" assembly="""">
			<properties>
				<property name=""Dto"" type=""TK2"" assembly="""">
					<properties>
						<property name=""ID"" type=""TK3"" assembly="""">1</property>
						<property name=""RevisionID"" type=""TK3"" assembly="""">1</property>
					</properties>
				</property>
			</properties>
		</item>
	</items>
	<!-- TypeDictionary : Don't edit anything in this section at all ! -->
	<typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib"">
		<items>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Common.RevisionDTO</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK3</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Int32</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages.PluginLifecycle.IPluginLocalMessage[]</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Messages.RevisionCreatedLocalMessage</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
		</items>
	</typedictionary>
</object>";
            var revisionCreatedLocalMessage = Deserialize(oldMessage);
            var dto = ((RevisionCreatedLocalMessage) revisionCreatedLocalMessage[0]).Dto;
            dto.ID.Should(Be.EqualTo(1), "dto.ID.Should(Be.EqualTo(1))");
        }

        [Test]
        public void ActionPatch()
        {
            const string oldMessage =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<object name="""" type=""TK0"" assembly="""">
	<!-- Data section : Don't edit any attributes ! -->
	<items>
		<item name=""0"" type=""TK1"" assembly="""">
			<properties>
				<property name=""EntityId"" type=""TK2"" assembly="""">0</property>
				<property name=""Dto"" type=""TK3"" assembly="""">
					<properties>
						<property name=""ID"" type=""TK2"" assembly="""">1</property>
						<property name=""RevisionID"" type=""TK2"" assembly="""">1</property>
					</properties>
				</property>
				<property name=""Children"" type=""TK4"" assembly="""">
					<items>
						<item name=""0"" type=""TK5"" assembly="""">
							<properties>
								<property name=""Status"" type=""TK6"" assembly="""">status</property>
							</properties>
						</item>
						<item name=""1"" type=""TK7"" assembly="""">
							<properties>
								<property name=""Comment"" type=""TK6"" assembly="""">comment</property>
							</properties>
						</item>
						<item name=""2"" type=""TK8"" assembly="""">
							<properties>
								<property name=""TimeSpent"" type=""TK9"" assembly="""">10</property>
							</properties>
						</item>
					</items>
				</property>
				<property name=""SagaId"" type=""TK10"" assembly="""">00000000-0000-0000-0000-000000000000</property>
			</properties>
		</item>
	</items>
	<!-- TypeDictionary : Don't edit anything in this section at all ! -->
	<typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib"">
		<items>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages.PluginLifecycle.IPluginLocalMessage[]</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Messages.AssignRevisionToEntityAction</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK3</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Common.RevisionDTO</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK8</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Comments.Actions.PostTimeAction</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK9</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Decimal</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK6</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.String</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK7</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Comments.Actions.PostCommentAction</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK4</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Collections.Generic.List`1[[Tp.Subversion.Comments.IAction, Tp.Subversion, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK5</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion.Comments.Actions.ChangeStatusAction</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Subversion</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Int32</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
			<item>
				<properties>
					<property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK10</property>
					<property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages"">
						<properties>
							<property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Guid</property>
							<property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property>
						</properties>
					</property>
				</properties>
			</item>
		</items>
	</typedictionary>
</object>";

            var revisionCreatedLocalMessage = Deserialize(oldMessage);
            var assignRevisionToEntityAction = revisionCreatedLocalMessage[0] as AssignRevisionToEntityAction;
            assignRevisionToEntityAction.Dto.ID.Should(Be.EqualTo(1), "assignRevisionToEntityAction.Dto.ID.Should(Be.EqualTo(1))");

            assignRevisionToEntityAction.Children[0].Should(Be.TypeOf<ChangeStatusAction>(),
                "assignRevisionToEntityAction.Children[0].Should(Be.TypeOf<ChangeStatusAction>())");
            ((ChangeStatusAction) assignRevisionToEntityAction.Children[0]).Status.Should(Be.EqualTo("status"),
                "((ChangeStatusAction)assignRevisionToEntityAction.Children[0]).Status.Should(Be.EqualTo(\"status\"))");

            assignRevisionToEntityAction.Children[1].Should(Be.TypeOf<PostCommentAction>(),
                "assignRevisionToEntityAction.Children[1].Should(Be.TypeOf<PostCommentAction>())");
            ((PostCommentAction) assignRevisionToEntityAction.Children[1]).Comment.Should(Be.EqualTo("comment"),
                "((PostCommentAction)assignRevisionToEntityAction.Children[1]).Comment.Should(Be.EqualTo(\"comment\"))");

            assignRevisionToEntityAction.Children[2].Should(Be.TypeOf<PostTimeAction>(),
                "assignRevisionToEntityAction.Children[2].Should(Be.TypeOf<PostTimeAction>())");
            ((PostTimeAction) assignRevisionToEntityAction.Children[2]).TimeSpent.Should(Be.EqualTo(10),
                "((PostTimeAction)assignRevisionToEntityAction.Children[2]).TimeSpent.Should(Be.EqualTo(10))");
        }

        [Test]
        public void CreateRevisionSaga()
        {
            const string oldXml =
                @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>Tp.Subversion.Workflow.CreateRevisionSagaData, Tp.Subversion</Type>
  <Version>2</Version>
  <string>{""__type"":""CreateRevisionSagaData:#Tp.Subversion.Workflow"",""&lt;Id&gt;k__BackingField"":""baf1a9ae-b874-44ea-827d-9fb400b48a78"",""&lt;OriginalMessageId&gt;k__BackingField"":""b4a0874e-6a92-4920-8e29-bea29a8ff361\\5498259"",""&lt;Originator&gt;k__BackingField"":""Tp.SubversionIntegration@TRUHTANOV"",""&lt;RevisionEntries&gt;k__BackingField"":[{""Action"":3,""Path"":""\/New\/test.txt""}],""&lt;RevisionFilesCreated&gt;k__BackingField"":0,""&lt;RevisionId&gt;k__BackingField"":0}</string>
</Value>
";
            var result =
                BlobSerializer.Deserialize(XDocument.Parse(oldXml), new TypeNameWithoutVersion(typeof(CreateRevisionSagaData)).Value) as
                    CreateRevisionSagaData;
            result.Id.ToString()
                .Should(Be.EqualTo("baf1a9ae-b874-44ea-827d-9fb400b48a78"),
                    "result.Id.ToString().Should(Be.EqualTo(\"baf1a9ae-b874-44ea-827d-9fb400b48a78\"))");
            result.RevisionId.Should(Be.EqualTo(0), "result.RevisionId.Should(Be.EqualTo(0))");
            result.RevisionFilesCreated.Should(Be.EqualTo(0), "result.RevisionFilesCreated.Should(Be.EqualTo(0))");
            result.RevisionEntries[0].Action.Should(Be.EqualTo(FileActionEnum.Modify),
                "result.RevisionEntries[0].Action.Should(Be.EqualTo(FileActionEnum.Modify))");
            result.RevisionEntries[0].Path.Should(Be.EqualTo(@"/New/test.txt"),
                "result.RevisionEntries[0].Path.Should(Be.EqualTo(@\"/New/test.txt\"))");
        }

        [Test, Ignore]
        public void AttachToEntitySaga()
        {
            const string oldXml =
                @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
	<Type>Tp.Subversion.Workflow.AttachToEntitySagaData, Tp.Subversion</Type>
	<Version>2</Version>
	<string>{""__type"":""AttachToEntitySagaData:#Tp.Subversion.Workflow"",""EntityId"":0,""Id"":""c2afc097-6ea3-4a0e-8170-9fb400bb8bbe"",""OriginalMessageId"":""b4a0874e-6a92-4920-8e29-bea29a8ff361\\5498359"",""Originator"":""Tp.SubversionIntegration@TRUHTANOV"",""RevisionDto"":{""&lt;ID&gt;k__BackingField"":null,""&lt;AuthorID&gt;k__BackingField"":null,""&lt;CommitDate&gt;k__BackingField"":null,""&lt;Description&gt;k__BackingField"":null,""&lt;PluginProfileID&gt;k__BackingField"":null,""&lt;ProjectID&gt;k__BackingField"":null,""&lt;ProjectName&gt;k__BackingField"":null,""&lt;RevisionID&gt;k__BackingField"":1,""&lt;SourceControlID&gt;k__BackingField"":null}}</string>
</Value>
";
            var result =
                BlobSerializer.Deserialize(XDocument.Parse(oldXml), new TypeNameWithoutVersion(typeof(AttachToEntitySagaData)).Value) as
                    AttachToEntitySagaData;
            result.Id.ToString()
                .Should(Be.EqualTo("c2afc097-6ea3-4a0e-8170-9fb400bb8bbe"),
                    "result.Id.ToString().Should(Be.EqualTo(\"c2afc097-6ea3-4a0e-8170-9fb400bb8bbe\"))");
            result.RevisionDto.ID.Should(Be.EqualTo(1), "result.RevisionDto.ID.Should(Be.EqualTo(1))");
        }
    }
}
