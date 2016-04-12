using Tp.I18n;

namespace Tp.Core
{
	public interface IPresentationErrorItem
	{
		string Type { get; }
		IFormattedMessage Message { get; }
	}
}
