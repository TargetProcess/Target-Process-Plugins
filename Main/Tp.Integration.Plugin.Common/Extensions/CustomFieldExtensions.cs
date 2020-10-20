using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;
using Tp.Model.Common;

namespace Tp.Integration.Plugin.Common.Extensions
{
    public static class CustomFieldExtensions
    {
        public static Try<ICustomFieldConfigHolder> GetConfig(this ICustomFieldDTO customField)
        {
            return Try.Create(() =>
            {
                var config = CustomFieldConfigSerializer.Instance.Deserialize(customField.Config);
                return (ICustomFieldConfigHolder) config;
            });
        }
    }
}
