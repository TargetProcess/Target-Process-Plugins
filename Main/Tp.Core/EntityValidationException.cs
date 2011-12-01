// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Text;

namespace Tp.Core
{
	/// <summary>
	/// The exception is fired when the entity contains the data which is invalid
	/// </summary>
	[Serializable]
	public class EntityValidationException : ApplicationException
	{
		private readonly IEnumerable<string> _messages;

		/// <summary>
		/// Initialize a new instance of the <see cref="EntityValidationException"/> class.
		/// </summary>
		/// <param name="messages">The messages.</param>
		public EntityValidationException(IEnumerable<string> messages)
		{
			_messages = new List<string>(messages);
		}

		public EntityValidationException(string message, Exception exception) : base(message, exception)
		{
			_messages = new[] {message};
		}

		/// <summary>
		/// Get collection of validation messages.
		/// </summary>
		public IEnumerable<string> Messages
		{
			get { return _messages; }
		}

		/// <summary>
		/// Get a message that describes the current exception.
		/// </summary>
		/// <value></value>
		/// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
		public override string Message
		{
			get
			{
				var stringBuilder = new StringBuilder();

				foreach (string message in _messages)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(message);
					stringBuilder.Append(".");
				}

				return stringBuilder.ToString();
			}
		}
	}
}