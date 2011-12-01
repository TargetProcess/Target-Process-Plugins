using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.Common.Domain
{
	public static class ProfileExtensions
	{
		public static PluginProfile ConvertToPluginProfile(this IProfileReadonly profile)
		{
			return new PluginProfile(profile.Name);
		}

		public static PluginProfileDto ConvertToDto(this IProfileReadonly profile)
		{
			return new PluginProfileDto { Name = profile.Name.Value, Settings = profile.Settings };
		}
	}
}