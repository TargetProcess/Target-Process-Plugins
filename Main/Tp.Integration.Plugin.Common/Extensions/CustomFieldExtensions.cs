using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Entities;

namespace Tp.Integration.Plugin.Common.Extensions
{
    public static class CustomFieldExtensions
    {
        public static Try<ICustomFieldConfigHolder> GetConfig(this ICustomFieldDTO customField)
        {
            return Try.Create(() =>
            {
                var config = customField.Config.Deserialize<CustomFieldConfigHolderDto>();
                return (ICustomFieldConfigHolder) config;
            });
        }
    }
}
