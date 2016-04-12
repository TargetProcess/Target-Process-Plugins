using System;
using Tp.I18n;

namespace Tp.Core
{
	public class LocalizedInvalidOperationException : InvalidOperationException, IFormattedMessageContainer
	{
		public LocalizedInvalidOperationException(IFormattedMessage formattedMessage)
			: base(formattedMessage.Value)
		{
			FormattedMessage = formattedMessage;
		}

		public IFormattedMessage FormattedMessage { get; }
	}
}
