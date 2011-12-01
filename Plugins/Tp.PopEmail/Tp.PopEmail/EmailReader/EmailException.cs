// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.PopEmailIntegration.EmailReader
{
	/// <summary>
	/// The exception that is thrown when email cannot be downloaded.
	/// </summary>
	[Serializable]
	public class EmailException : ApplicationException
	{
		public EmailException()
		{
		}

		public EmailException(string message) : base(message)
		{
		}

		public EmailException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public EmailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}