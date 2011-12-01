// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Xml.Linq;
using NServiceBus;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Gateways
{
	[Serializable]
	public class PostponedMessage
	{
		public PostponedMessage()
		{
		}

		public PostponedMessage(IMessage message)
		{
			_serializedMsg = BlobSerializer.Serialize(message);
			_type = message.GetType();
		}

		private readonly XDocument _serializedMsg;
		private readonly Type _type;

		public IMessage Retrieve()
		{
			return (IMessage) BlobSerializer.Deserialize(_serializedMsg, _type);
		}
	}
}