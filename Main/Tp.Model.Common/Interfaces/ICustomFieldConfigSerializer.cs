using Tp.Model.Common.Entities.CustomField;

namespace Tp.Model.Common.Interfaces
{
    public interface ICustomFieldConfigSerializer
    {
        string Serialize(CustomFieldConfig config);
        CustomFieldConfig Deserialize(string stringConfig);
    }
}
