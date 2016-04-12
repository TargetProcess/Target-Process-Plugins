using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Xml.Serialization;
using NServiceBus.Unicast.Transport;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	struct MessageAccount
	{
		public static readonly MessageAccount Empty = new MessageAccount(string.Empty, string.Empty);
		private readonly string _name;
		private readonly string _command;

		public MessageAccount(string name, string command)
		{
			_name = name;
			_command = command;
		}

		public string Name
		{
			get { return _name; }
		}

		public string Command
		{
			get { return _command; }
		}
	}

	internal class MessageAccountParser
	{
		public static readonly MessageAccountParser Instance = new MessageAccountParser();

		private readonly XmlSerializer _serializer;

		private MessageAccountParser()
		{
			_serializer = new XmlSerializer(typeof(List<HeaderInfo>));
		}

		public MessageAccount Parse(IEnumerable<HeaderInfo> parts)
		{
			var name = string.Empty;
			var command = string.Empty;
			foreach (HeaderInfo part in parts)
			{
				var key = part.Key;
				if (key == BusExtensions.ACCOUNTNAME_KEY)
				{
					name = part.Value;
				}
				else if (key == "Command")
				{
					command = part.Value;
				}
			}
			return new MessageAccount(name, command);
		}

		public MessageAccount Parse(Message m)
		{
			if (m == null || m.Extension.Length == 0)
			{
				return MessageAccount.Empty;
			}
			using (var s = new MemoryStream(m.Extension))
			{
				var parts = (List<HeaderInfo>) _serializer.Deserialize(s);
				return Parse(parts);
			}
		}
	}
}
