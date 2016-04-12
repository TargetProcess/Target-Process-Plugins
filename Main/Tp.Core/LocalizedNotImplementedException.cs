using System;
using Tp.I18n;

namespace Tp.Core
{
	public class LocalizedNotImplementedException : NotImplementedException, IFormattedMessageContainer
	{
		public LocalizedNotImplementedException(IFormattedMessage formattedMessage)
			: base(formattedMessage.Value)
		{
			FormattedMessage = formattedMessage;
		}

		public IFormattedMessage FormattedMessage { get; }

		public static IFormattedMessage DefaultMessage => "This method is not implemented. You need to do this another way.".Localize();
	}
}
