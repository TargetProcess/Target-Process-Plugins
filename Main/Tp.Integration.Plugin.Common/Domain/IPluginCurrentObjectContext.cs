namespace Tp.Integration.Plugin.Common.Domain
{
	interface IPluginCurrentObjectContext : IPluginCurrentObjectContextReadonly
	{
		new IProfileCollection ProfileCollection { get; }
		new IProfile Profile { get; }
	}
}