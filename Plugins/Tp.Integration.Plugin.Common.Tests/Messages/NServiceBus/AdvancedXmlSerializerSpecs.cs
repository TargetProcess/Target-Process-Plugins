// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using NBehave.Narrator.Framework;
using NServiceBus;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.ServiceBus.Serialization;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Messages.NServiceBus
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class AdvancedXmlSerializerSpecs
	{
		private SampleMessage _message;

		private XmlDocument _serialized;

		private XmlSerializer _serializer;

		private XmlDeserializer _deserializer;

		[SetUp]
		public void Init()
		{
			_serializer = new XmlSerializer();
			_deserializer = new XmlDeserializer();
		}

		[Test]
		public void ShouldSerializeSampleMessage()
		{
			@"Given sample message with name 'Name'
				When sample message serialized to xml
				Then deserialized from xml sample message name is 'Name'
					And deserialized from xml sample message saga id is equal to sample message saga id"
				.Execute();
		}

		[Test, Ignore("Optimize array of value types serialization")]
		public void ShouldSerializeValueTypeArrays()
		{
			var message = new SampleMessage {Bytes = new byte[] {1, 2, 3}};
			var bytesEncoded = Convert.ToBase64String(message.Bytes);

			var serialized = _serializer.Serialize(message);
			serialized.InnerText.Should(Is.StringContaining(bytesEncoded), "serialized.InnerText.Should(Is.StringContaining(bytesEncoded))");

			var deserialized = (SampleMessage) _deserializer.Deserialize(serialized);
			deserialized.Bytes.Should(Is.EquivalentTo(message.Bytes), "deserialized.Bytes.Should(Is.EquivalentTo(message.Bytes))");
		}

		[Test]
		public void CorrectReferenceSerialization()
		{
			var dto = new DataTransferObject();
			var message1 = new CreateCommand {Dto = dto};
			var message2 = new CreateCommand {Dto = dto};

			var serialized = _serializer.Serialize(new[] {message1, message2});

			var deserialized = (CreateCommand[]) _deserializer.Deserialize(serialized);
			deserialized[0].Dto.Should(Is.Not.Null, "deserialized[0].Dto.Should(Is.Not.Null)");
			deserialized[1].Dto.Should(Is.Not.Null, "deserialized[1].Dto.Should(Is.Not.Null)");
		}

		[Test]
		public void DatesShouldBeEncodedUsingInvariantCulture()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-BE");

			try
			{
				var message = new MessageWithDates {MyDate = DateTime.Today};
				var serialized = _serializer.Serialize(message);

				Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
				var deserialized = (MessageWithDates) _deserializer.Deserialize(serialized);
				deserialized.MyDate.Should(Be.EqualTo(message.MyDate), "deserialized.MyDate.Should(Be.EqualTo(message.MyDate))");
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		public class MessageWithDates
		{
			public DateTime MyDate { get; set; }
		}

		public class MyMessage
		{
			public MyMessage Msg { get; set; }
		}

		[Test]
		public void CorrectCyclicReferenceSerialization()
		{
			var message = new MyMessage();
			message.Msg = message;

			var serialized = _serializer.Serialize(message);

			var deserialized = (MyMessage) _deserializer.Deserialize(serialized);
			deserialized.Should(Is.Not.Null, "deserialized.Should(Is.Not.Null)");
			deserialized.Msg.Should(Is.Null, "deserialized.Msg.Should(Is.Null)");
		}

		public class DateTimeMessage : IMessage
		{
			public DateTime CreateDate { get; set; }
		}

		[Test]
		public void ShouldSerializeDateTime()
		{
			var dateTime = DateTime.Now;
			var message = new DateTimeMessage {CreateDate = dateTime};
			var memoryStream = new MemoryStream();
			new AdvancedXmlSerializer().Serialize(new IMessage[] {message}, memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);

			var deserialized = new AdvancedXmlSerializer().Deserialize(memoryStream);
			((DateTimeMessage) deserialized[0]).CreateDate.ToString().Should(Be.EqualTo(dateTime.ToString()), "((DateTimeMessage) deserialized[0]).CreateDate.ToString().Should(Be.EqualTo(dateTime.ToString()))");
		}

		[Test]
		public void ShouldSerializeSpecialSymbols()
		{
			var memoryStrem = new MemoryStream();
			//Special character with hexadecimal value 0x15 is inserted in Name. It is not empty.
			var sampleMessage = new SampleMessage {Name = ""};
			new AdvancedXmlSerializer().Serialize(new IMessage[] {sampleMessage}, memoryStrem);
			memoryStrem.Seek(0, SeekOrigin.Begin);
			var res = new AdvancedXmlSerializer().Deserialize(memoryStrem)[0] as SampleMessage;
			res.Name.Should(Be.Not.Empty, "res.Name.Should(Be.Not.Empty)");
		}

		[Test]
		public void ShouldDeserializeAfterAddingNewProperties()
		{
			const string oldSerializedMessage = @"<?xml version=""1.0"" encoding=""utf-8""?><object name="""" type=""TK0"" assembly=""""><!-- Data section : Don't edit any attributes ! --><items><item name=""0"" type=""TK1"" assembly=""""><properties><property name=""Dto"" type=""TK2"" assembly=""""><properties><property name=""Name"" type=""TK3"" assembly="""">US Name</property></properties></property><property name=""SagaId"" type=""TK4"" assembly="""">00000000-0000-0000-0000-000000000000</property></properties></item></items><!-- TypeDictionary : Don't edit anything in this section at all ! --><typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib""><items><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages.EntityLifecycle.Messages.UserStoryCreatedMessage[]</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK3</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.String</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages.EntityLifecycle.Messages.UserStoryCreatedMessage</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Common.UserStoryDTO</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Messages</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK4</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Guid</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property></properties></property></properties></item></items></typedictionary></object>";
			var deserialized = _deserializer.Deserialize(new XmlDocument {InnerXml = oldSerializedMessage});
			((UserStoryCreatedMessage[])deserialized).Single().Dto.CustomFieldsMetaInfo.Should(Be.Null, "((UserStoryCreatedMessage[])deserialized).Single().Dto.CustomFieldsMetaInfo.Should(Be.Null)");
		}

		[Test]
		public void SerializeCustomFieldsMetaCorrectly()
		{
			var dto = new UserStoryDTO
				{
					CustomFieldsMetaInfo = new[] {new Field {FieldType = FieldTypeEnum.Text, Value = "Value"}}
				};

			var message = new UserStoryCreatedMessage {Dto = dto};
			var serialized = _serializer.Serialize(message);
			var deserialized = _deserializer.Deserialize(serialized);
			var meta = ((UserStoryCreatedMessage) deserialized).Dto.CustomFieldsMetaInfo;
			meta.Single().FieldType.Should(Be.EqualTo(FieldTypeEnum.Text), "meta.Single().FieldType.Should(Be.EqualTo(FieldTypeEnum.Text))");
		}

		[Given("sample message with name '$sampleMessageName'")]
		public void CreateSampleMessageName(string sampleMessageName)
		{
			_message = new SampleMessage {Name = sampleMessageName, SagaId = Guid.NewGuid()};
		}

		[When("sample message serialized to xml")]
		public void SerializeSampleMessageToXml()
		{
			var xmlSerializer = new XmlSerializer();
			_serialized = xmlSerializer.Serialize(_message);
		}

		[Then("deserialized from xml sample message name is '$sampleMessageName'")]
		public void DeserializeSampleMessageFromXmlAndAssertThatNameIs(string sampleMessageName)
		{
			var deserializer = new XmlDeserializer();

			var deserialized = (SampleMessage) deserializer.Deserialize(_serialized);
			deserialized.Name.Should(Is.EqualTo(sampleMessageName), "deserialized.Name.Should(Is.EqualTo(sampleMessageName))");
		}

		[Then("deserialized from xml sample message saga id is equal to sample message saga id")]
		public void DeserializedSampleMessageSagaIdIsEqualToSampleMessageSagaId()
		{
			var deserializer = new XmlDeserializer();

			var deserialized = (SampleMessage) deserializer.Deserialize(_serialized);
			deserialized.SagaId.Should(Is.EqualTo(_message.SagaId), "deserialized.SagaId.Should(Is.EqualTo(_message.SagaId))");
		}

		[Test]
		public void SerializationShouldBeForwardCompatibleWhenAddingNewFieldOfPrimitiveType()
		{
			//'messageFromNewSdk' variable is a serialized instance of class 
			//private class SampleSerializationMessage
			//{
			//	public string StringField { get; set; }
			//	public int IntField { get; set; }
			//}
			var messageFromNewSdk =
				@"<?xml version=""1.0"" encoding=""utf-8""?><object name="""" type=""TK0"" assembly=""""><!-- Data section : Don't edit any attributes ! --><properties><property name=""StringField"" type=""TK1"" assembly="""">123</property><property name=""IntField"" type=""TK2"" assembly="""">12</property></properties><!-- TypeDictionary : Don't edit anything in this section at all ! --><typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib""><items><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Plugin.Common.Tests.Messages.NServiceBus.AdvancedXmlSerializerSpecs+SampleSerializationMessage</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Plugin.Common.Tests</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.Int32</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.String</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property></properties></property></properties></item></items></typedictionary></object>";

			var xml = new XmlDocument();
			xml.LoadXml(messageFromNewSdk);
			var res = (SampleSerializationMessage) _deserializer.Deserialize(xml);
			res.StringField.Should(Be.EqualTo("123"), $"{nameof(res.StringField)} field is not deserialized correctly");
			res.DoubleField.Should(Be.EqualTo(0), $"{nameof(res.DoubleField)} field has not default value");
		}

		[Test]
		public void SerializationShouldBeForwardCompatibleWhenAddingNewFieldOfCustomType()
		{
			//'messageFromNewSdk' variable is a serialized instance of class 
			//private class SampleSerializationMessage
			//{
			//	public string StringField { get; set; }
			//	public SampleEnum SampleEnumField { get; set; }
			//}

			//private enum SampleEnum
			//{
			//	None,
			//	SomeValue
			//}

			var messageFromNewSdk =
				@"<?xml version=""1.0"" encoding=""utf-8""?><object name="""" type=""TK0"" assembly=""""><!-- Data section : Don't edit any attributes ! --><properties><property name=""StringField"" type=""TK1"" assembly="""">123</property><property name=""SampleEnumField"" type=""TK2"" assembly="""">SomeValue</property></properties><!-- TypeDictionary : Don't edit anything in this section at all ! --><typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib""><items><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK2</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Plugin.Common.Tests.Messages.NServiceBus.AdvancedXmlSerializerSpecs+SampleEnum</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Plugin.Common.Tests</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK0</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Plugin.Common.Tests.Messages.NServiceBus.AdvancedXmlSerializerSpecs+SampleSerializationMessage</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">Tp.Integration.Plugin.Common.Tests</property></properties></property></properties></item><item><properties><property name=""Key"" type=""System.String"" assembly=""mscorlib"">TK1</property><property name=""Value"" type=""Tp.Integration.Messages.ServiceBus.Serialization.TypeInfo"" assembly=""Tp.Integration.Messages""><properties><property name=""TypeName"" type=""System.String"" assembly=""mscorlib"">System.String</property><property name=""AssemblyName"" type=""System.String"" assembly=""mscorlib"">mscorlib</property></properties></property></properties></item></items></typedictionary></object>";

			var xml = new XmlDocument();
			xml.LoadXml(messageFromNewSdk);
			var res = (SampleSerializationMessage)_deserializer.Deserialize(xml);
			res.StringField.Should(Be.EqualTo("123"), $"{nameof(res.StringField)} field is not deserialized correctly");
			res.DoubleField.Should(Be.EqualTo(0), $"{nameof(res.DoubleField)} field has not default value");
		}


		private class SampleSerializationMessage
		{
			public string StringField { get; set; }
			public double DoubleField { get; set; }
		}
	}
}