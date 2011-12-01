// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using NBehave.Narrator.Framework;
using NServiceBus;
using NServiceBus.Saga;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.SagaPersister;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.SagaPersister
{
	[TestFixture]
	[ActionSteps]
	public class TpDatabaseSagaPersisterSpecs : SqlPersisterSpecBase
	{
		private TestSaga _saga;
		private Guid _sagaId;

		protected override void InitializeContainer(IInitializationExpression initializationExpression)
		{
			base.InitializeContainer(initializationExpression);
			initializationExpression.For<IPluginContext>().HybridHttpOrThreadLocalScoped().Use<PluginContext>();
			initializationExpression.For<ISagaPersister>().HybridHttpOrThreadLocalScoped().Use<TpDatabaseSagaPersister>();
		}

		[Test]
		public void ShouldRetrieveSagaForProfile()
		{
			@"
			Given plugin in local database mode
				And store account 'Account1' with profiles: Profile1_1, Profile1_2
				And test saga with value 'testValue' started by profile 'Profile1_1'
			When test saga data is persisted
			Then saga persister should contain test saga with value 'testValue' for profile 'Profile1_1'
				And saga persister should contain no sagas for profile 'Profile1_2'"
				.Execute(In.Context<PluginSqlPersisterSpecs>().And<TpDatabaseSagaPersisterSpecs>());
		}

		[Test]
		public void ShouldCompleteSaga()
		{
			@"
			Given plugin in local database mode
				And store account 'Account1' with profiles: Profile1_1
				And test saga with value 'testValue' started by profile 'Profile1_1'
			When test saga data is persisted
				And test saga is completed
			Then saga persister should contain no sagas for profile 'Profile1_1'"
				.Execute(In.Context<PluginSqlPersisterSpecs>().And<TpDatabaseSagaPersisterSpecs>());
		}

		[Test]
		public void ShouldUpdateSaga()
		{
			@"
			Given plugin in local database mode
				And store account 'Account1' with profiles: Profile1_1
				And test saga with value 'testValue' started by profile 'Profile1_1'
			When test saga data is persisted
				And test saga value is updated to 'testValueUpdated'
			Then saga persister should contain test saga with value 'testValueUpdated' for profile 'Profile1_1'"
				.Execute(In.Context<PluginSqlPersisterSpecs>().And<TpDatabaseSagaPersisterSpecs>());
		}

		[Test]
		public void ShouldRetrieveSagaByProperty()
		{
			@"
			Given plugin in local database mode
				And store account 'Account1' with profiles: Profile1_1
				And test saga with value 'testValue' started by profile 'Profile1_1'
			When test saga data is persisted
			Then saga persister should be able to retrieve test saga by 'TestValue' property with value 'testValue' for profile 'Profile1_1'"
				.Execute(In.Context<PluginSqlPersisterSpecs>().And<TpDatabaseSagaPersisterSpecs>());
		}

		[Test]
		public void ShouldRetrieveSagaByIdPropertyFast()
		{
			@"
			Given plugin in local database mode
				And store account 'Account1' with profiles: Profile1_1
				And test saga with id '098d0873-71bb-4dcf-99f5-059024aa79b8' started by profile 'Profile1_1'
			When test saga data is persisted
			Then saga persister should be able to retrieve test saga by id property with value '098d0873-71bb-4dcf-99f5-059024aa79b8' for profile 'Profile1_1'"
				.Execute(In.Context<PluginSqlPersisterSpecs>().And<TpDatabaseSagaPersisterSpecs>());
		}

		[Given("test saga with value '$sagaDataValue' started by profile '$profileName'")]
		public void StartSagaByProfile(string sagaDataValue, string profileName)
		{
			ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.PROFILENAME_KEY] = profileName;
			_sagaId = Guid.NewGuid();
			_saga = new TestSaga {Data = new TestSagaData {TestValue = sagaDataValue, Id = _sagaId}};
		}

		[Given("test saga with id '$sagaId' started by profile '$profileName'")]
		public void StartSagaSaga(string sagaId, string profileName)
		{
			ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.PROFILENAME_KEY] = profileName;
			_sagaId = new Guid(sagaId);
			_saga = new TestSaga {Data = new TestSagaData {Id = _sagaId}};
		}

		[When("test saga data is persisted")]
		public void PersistSaga()
		{
			ObjectFactory.GetInstance<ISagaPersister>().Save(_saga.Data);
		}

		[When("test saga is completed")]
		public void CompleteTestSaga()
		{
			ObjectFactory.GetInstance<ISagaPersister>().Complete(_saga.Data);
		}

		[When("test saga value is updated to '$sagaDataValueUpdated'")]
		public void UpdateTestSagaValue(string sagaDataValueUpdated)
		{
			_saga.Data.TestValue = sagaDataValueUpdated;
			ObjectFactory.GetInstance<ISagaPersister>().Update(_saga.Data);
		}

		[Then("saga persister should contain test saga with value '$sagaDataValue' for profile '$profileName'")]
		public void SagaPersisterShouldContainSagaForProfile(string sagaDataValue, string profileName)
		{
			ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.PROFILENAME_KEY] = profileName;
			ObjectFactory.GetInstance<TpDatabaseSagaPersister>().Get<TestSagaData>(_sagaId).TestValue.Should(
				Be.EqualTo(sagaDataValue));
		}

		[Then("saga persister should contain no sagas for profile '$profileName'")]
		public void SagaPersisterShouldNotContainSagaForProfile(string profileName)
		{
			ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.PROFILENAME_KEY] = profileName;
			ObjectFactory.GetInstance<TpDatabaseSagaPersister>().Get<TestSagaData>(_sagaId).Should(Be.Null);
		}

		[Then(
			"saga persister should be able to retrieve test saga by '$propertyName' property with value '$propertyValue' for profile '$profileName'"
			)]
		public void SagaShouldBeRetrievedByProperty(string propertyName, string propertyValue, string profileName)
		{
			ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.PROFILENAME_KEY] = profileName;
			ObjectFactory.GetInstance<TpDatabaseSagaPersister>().Get<TestSagaData>(propertyName, propertyValue).Should(
				Be.Not.Null);
		}

		[Then(
			"saga persister should be able to retrieve test saga by id property with value '$propertyValue' for profile '$profileName'"
			)]
		public void SagaShouldBeRetrievedByIdProperty(string propertyValue, string profileName)
		{
			ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.PROFILENAME_KEY] = profileName;
			ObjectFactory.GetInstance<TpDatabaseSagaPersister>().Get<TestSagaData>("Id", new Guid(propertyValue)).Should(
				Be.Not.Null);
		}

		[Test]
		public void BlobSerializerTest()
		{
			var res = BlobSerializer.Serialize(new TestSagaData {TestValue = "testValue"});
			res.Should(Be.Not.Null);

			var des = BlobSerializer.Deserialize(res, typeof (ISagaEntity).Name);
			des.Should(Be.Not.Null);
			des.Should(Be.TypeOf<TestSagaData>());
			((TestSagaData) des).TestValue.Should(Be.EqualTo("testValue"));
		}

		[Test]
		public void EnsureJsonBlobSerializerVersion2WillNotBeChanged()
		{
			const string expected =
				@"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
				  <Type>Tp.Integration.Plugin.Common.Tests.Common.SagaPersister.TestSagaData, Tp.Integration.Plugin.Common.Tests</Type>
				  <Version>2</Version>
				  <string>{""__type"":""TestSagaData:#Tp.Integration.Plugin.Common.Tests.Common.SagaPersister"",""Id"":""00000000-0000-0000-0000-000000000000"",""OriginalMessageId"":null,""Originator"":null,""TestValue"":""Value""}</string>
				</Value>";

			var expectedXml = XDocument.Parse(expected);

			var result = BlobSerializer.Serialize(new TestSagaData {TestValue = "Value"});

			result.ToString().Should(Be.EqualTo(expectedXml.ToString()));
			JsonBlobSerializer.VERSION.Should(Be.EqualTo("2"));
		}

		[Test]
		public void EnsureXmlSerializerAppliedToOldSerializedData()
		{
			const string toDeserialize =
				@"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
										  <Type>Tp.Integration.Plugin.Common.Tests.Common.SagaPersister.TestSagaData, Tp.Integration.Plugin.Common.Tests</Type>
										  <TestSagaData>
											<TestValue>testValue</TestValue>
											<Id>00000000-0000-0000-0000-000000000000</Id>
										  </TestSagaData>
										</Value>";

			var toDeserializeXml = XDocument.Parse(toDeserialize);

			var profile = BlobSerializer.Deserialize(toDeserializeXml, typeof (ISagaEntity).Name);

			var typedProfile = profile as TestSagaData;
			typedProfile.Id.Should(Be.EqualTo(Guid.Empty));
			typedProfile.TestValue.Should(Be.EqualTo("testValue"));
		}

		[Test, ExpectedException(typeof (ApplicationException))]
		public void ShouldThrowExceptionIfNoSerializersApplied()
		{
			var toDeserialize = XDocument.Parse("<SomeTag>Value</SomeTag>");

			BlobSerializer.Deserialize(toDeserialize, typeof (ISagaEntity).Name);
		}
	}

	[Serializable, DataContract]
	public class TestSagaData : ISagaEntity
	{
		[DataMember]
		public string TestValue { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Originator { get; set; }

		[DataMember]
		public string OriginalMessageId { get; set; }
	}

	public class TestSaga : TpSaga<TestSagaData>
	{
	}
}