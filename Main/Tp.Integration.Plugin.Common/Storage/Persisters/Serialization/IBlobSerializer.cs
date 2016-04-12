using System.Xml.Linq;

namespace Tp.Integration.Plugin.Common.Storage.Persisters.Serialization
{
	public interface IBlobSerializer
	{
		object Deserialize(XDocument stateData, string keyType);
		XDocument Serialize(object value);
	}
}