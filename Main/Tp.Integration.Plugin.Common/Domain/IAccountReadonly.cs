using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
    public interface IAccountReadonly
    {
        AccountName Name { get; }
        IProfileCollectionReadonly Profiles { get; }
    }
}
