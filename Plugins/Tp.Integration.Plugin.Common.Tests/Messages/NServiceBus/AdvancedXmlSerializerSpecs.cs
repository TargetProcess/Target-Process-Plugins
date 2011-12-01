// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using NBehave.Narrator.Framework;
using NServiceBus;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.ServiceBus.Serialization;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Messages.NServiceBus
{
	[TestFixture, ActionSteps]
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
			serialized.InnerText.Should(Is.StringContaining(bytesEncoded));

			var deserialized = (SampleMessage) _deserializer.Deserialize(serialized);
			deserialized.Bytes.Should(Is.EquivalentTo(message.Bytes));
		}

		[Test]
		public void CorrectReferenceSerialization()
		{
			var dto = new DataTransferObject();
			var message1 = new CreateCommand {Dto = dto};
			var message2 = new CreateCommand {Dto = dto};

			var serialized = _serializer.Serialize(new[] {message1, message2});

			var deserialized = (CreateCommand[]) _deserializer.Deserialize(serialized);
			deserialized[0].Dto.Should(Is.Not.Null);
			deserialized[1].Dto.Should(Is.Not.Null);
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
				deserialized.MyDate.Should(Be.EqualTo(message.MyDate));
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
			deserialized.Should(Is.Not.Null);
			deserialized.Msg.Should(Is.Null);
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
			((DateTimeMessage) deserialized[0]).CreateDate.ToString().Should(Be.EqualTo(dateTime.ToString()));
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
			res.Name.Should(Be.Not.Empty);
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
			deserialized.Name.Should(Is.EqualTo(sampleMessageName));
		}

		[Then("deserialized from xml sample message saga id is equal to sample message saga id")]
		public void DeserializedSampleMessageSagaIdIsEqualToSampleMessageSagaId()
		{
			var deserializer = new XmlDeserializer();

			var deserialized = (SampleMessage) deserializer.Deserialize(_serialized);
			deserialized.SagaId.Should(Is.EqualTo(_message.SagaId));
		}
	}
}