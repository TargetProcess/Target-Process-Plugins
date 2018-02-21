namespace Tp.Integration.Plugin.Common.Domain
{
    interface IPluginCurrentObjectContextReadonly
    {
        IProfileReadonly Profile { get; }
        IProfileCollectionReadonly ProfileCollection { get; }
    }
}
