using System.Runtime.Serialization;

namespace Tp.Search.Model
{
	[DataContract]
	class IndexProgress
	{
		[DataMember]
		public int LastGeneralId { get; set; }
		[DataMember]
		public int LastCommentId { get; set; }
		[DataMember]
		public int LastTestStepId { get; set; }
	}
}