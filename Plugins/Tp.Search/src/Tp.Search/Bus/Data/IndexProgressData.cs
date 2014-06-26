using System.Runtime.Serialization;

namespace Tp.Search.Bus.Data
{
	[DataContract]
	public class IndexProgressData
	{
		[DataMember]
		public double CompleteInPercents { get; set; }
	}
}