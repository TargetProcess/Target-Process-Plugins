// -----------------------------------------------------------------------------------
// Use it as you please, but keep this header.
// Author : Marcus Deecke, 2006
// Web    : www.yaowi.com
// Email  : code@yaowi.com
// -----------------------------------------------------------------------------------
namespace Tp.Integration.Messages.ServiceBus.Serialization
{
  public interface IXmlSerializationTag
  {
    string ASSEMBLY_TAG { get; }
    string INDEX_TAG { get; }
    string ITEM_TAG { get; }
    string ITEMS_TAG { get; }
    string NAME_ATT_KEY_TAG { get; }
    string NAME_ATT_VALUE_TAG { get; }
    string NAME_TAG { get; }
    string OBJECT_TAG { get; }
    string PROPERTIES_TAG { get; }
    string PROPERTY_TAG { get; }
    string TYPE_DICTIONARY_TAG { get; }
    string TYPE_TAG { get; }
    string GENERIC_TYPE_ARGUMENTS_TAG { get; }
    string CONSTRUCTOR_TAG { get;}
    string BINARY_DATA_TAG { get;}
  }
}