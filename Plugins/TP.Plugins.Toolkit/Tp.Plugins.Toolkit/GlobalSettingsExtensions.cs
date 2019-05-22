using System;
using Tp.Integration.Common;

namespace Tp.Plugins.Toolkit
{
    public static class GlobalSettingsExtensions
    {
        public static string GetAccountName(this IGlobalSettingDTO globalSettingDto, string accountName)
        {
            return globalSettingDto?.AppHostAndPath.Split(new[] { Uri.SchemeDelimiter }, StringSplitOptions.None)[1].TrimEnd('/', '\\') ?? accountName;
        }

        public static string GetUrl(this IGlobalSettingDTO globalSettingDto, string accountName)
        {
            return globalSettingDto != null ? globalSettingDto.AppHostAndPath : "https://" + accountName;
        }

        public static string GetEntityUrl(this IGlobalSettingDTO globalSettingDto, string accountName, int? id)
        {
            var url = globalSettingDto.GetUrl(accountName).TrimEnd('/');
            return id.HasValue ? url + "/entity/" + id : url;
        }
    }
}
