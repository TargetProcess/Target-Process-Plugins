using Tp.Integration.Common;

namespace Tp.Integration.Messages.Entities
{
    public interface ICustomFieldInfo : ICustomFieldConfigHolder
    {
        string Name { get; }
        FieldTypeEnum FieldType { get; }
        string EntityFieldName { get; }
        string Value { get; }
        int? EntityTypeID { get; }
    }
}
