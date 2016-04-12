using System;
using System.Xml.Linq;
using NUnit.Framework;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;
using Tp.Search.Bus.Serialization;
using Tp.Search.Bus.Workflow;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
	[TestFixture]
    [Category("PartPlugins1")]
	class SagaSerializationTests
	{
		[Test]
		public void Test()
		{
			const string oldSaga = "<Value xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Type>Tp.Search.Bus.Workflow.IndexExistingEntitiesSagaData, Tp.Search</Type><Version>2</Version><string>{\"__type\":\"IndexExistingEntitiesSagaData:#Tp.Search.Bus.Workflow\",\"&lt;CommentsRetrievedCount&gt;k__BackingField\":0,\"&lt;GeneralsRetrievedCount&gt;k__BackingField\":0,\"&lt;Id&gt;k__BackingField\":\"4981ab85-8f0d-4a37-aa0b-a1c0010f88f0\",\"&lt;OriginalMessageId&gt;k__BackingField\":\"15a0e019-ace7-42b5-bc7d-f969c7270432\\1842090\",\"&lt;Originator&gt;k__BackingField\":\"Tp.Search@SHOTKIN\",\"&lt;OuterSagaId&gt;k__BackingField\":\"95a6b77a-1b9a-49cb-8e7c-a1c0010f8429\",\"&lt;SkipComments&gt;k__BackingField\":0,\"&lt;SkipGenerals&gt;k__BackingField\":30}</string></Value>\"";
			var path = new IndexExistingEntitiesSagaPreviousVersionCorrecter();
			path.NeedToApply(oldSaga).Should(Be.True, "path.NeedToApply(oldSaga).Should(Be.True)");
			var text = path.Apply(oldSaga);
			var deserialized = BlobSerializer.Deserialize(XDocument.Parse(text), string.Empty);
			var typed = deserialized as IndexExistingEntitiesSagaData;
			typed.Should(Be.Not.Null, "typed.Should(Be.Not.Null)");
			typed.Id.Should(Be.EqualTo(Guid.Parse("4981ab85-8f0d-4a37-aa0b-a1c0010f88f0")), "typed.Id.Should(Be.EqualTo(Guid.Parse(\"4981ab85-8f0d-4a37-aa0b-a1c0010f88f0\")))");
		}

		[Test]
		public void DeserializeProjectProcessChangedSagaDataAfterAddingFields()
		{
			const string oldSaga = @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>Tp.Search.Bus.Workflow.ProjectProcessChangedSagaData, Tp.Search</Type>
  <Version>2</Version>
  <string>{""__type"":""ProjectProcessChangedSagaData:#Tp.Search.Bus.Workflow"",""Id"":""00000000-0000-0000-0000-000000000000"",""OriginalMessageId"":null,""Originator"":null,""ProjectId"":1,""SkipGenerals"":10}</string>
</Value>";

			var deserialized = (ProjectProcessChangedSagaData)BlobSerializer.Deserialize(XDocument.Parse(oldSaga), string.Empty);
			deserialized.ProjectId.Should(Be.EqualTo(1), "deserialized.ProjectId.Should(Be.EqualTo(1))");
			deserialized.AssignablesRetrievedCount.Should(Be.EqualTo(0), "deserialized.AssignablesRetrievedCount.Should(Be.EqualTo(0))");
		}

		[Test]
		public void DeserializeGeneralProjectChangedSagaDataAfterAddingFields()
		{
			const string oldSaga = @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>Tp.Search.Bus.Workflow.GeneralProjectChangeSagaData, Tp.Search</Type>
  <Version>2</Version>
  <string>{""__type"":""GeneralProjectChangeSagaData:#Tp.Search.Bus.Workflow"",""CommentsCurrentDataWindowSize"":0,""CommentsRetrievedCount"":0,""GeneralId"":147,""Id"":""534a26de-645c-4e66-9f83-a43b0152e933"",""OriginalMessageId"":""9753de6d-3e2d-4d1b-9b60-8d449f5592cc\\3914349"",""Originator"":""Tp.Search@D-KANASHEVICH"",""ProjectId"":13}</string>
</Value>";

			var deserialized = (GeneralProjectChangeSagaData)BlobSerializer.Deserialize(XDocument.Parse(oldSaga), string.Empty);
			deserialized.ProjectId.Should(Be.EqualTo(13), "deserialized.ProjectId.Should(Be.EqualTo(13))");
			deserialized.TestStepsRetrievedCount.Should(Be.EqualTo(0), "deserialized.TestStepsRetrievedCount.Should(Be.EqualTo(0))");
		}
	}
}