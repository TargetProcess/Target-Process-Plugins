using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	public interface IProfileReadonly: IStorageRepository
	{
		ProfileName Name { get; }
		object Settings { get; }
		bool Initialized { get; }
	}
}