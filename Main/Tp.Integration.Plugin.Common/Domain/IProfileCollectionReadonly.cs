using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	public interface IProfileCollectionReadonly : IEnumerable<IProfileReadonly>
	{
		IProfileReadonly this[ProfileName profileName] { get; }
	}
}