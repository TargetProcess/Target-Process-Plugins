using System.Runtime.Serialization;
using System.Text;

namespace Tp.Search.Bus.Data
{
    [DataContract]
    public class QueryResultData
    {
        [DataMember]
        public string QueryString { get; set; }

        [DataMember]
        public string[] GeneralIds { get; set; }

        [DataMember]
        public string[] AssignableIds { get; set; }

        [DataMember]
        public string[] TestStepIds { get; set; }

        [DataMember]
        public string[] ImpedimentIds { get; set; }

        [DataMember]
        public string[] CommentIds { get; set; }

        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public IndexProgressData IndexProgressData { get; set; }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.AppendFormat("QueryString = {0}", QueryString).AppendLine()
                .AppendFormat("GeneralIds = {0}", string.Join(",", GeneralIds)).AppendLine()
                .AppendFormat("AssignableIds = {0}", string.Join(",", AssignableIds)).AppendLine()
                .AppendFormat("TestStepIds = {0}", string.Join(",", TestStepIds)).AppendLine()
                .AppendFormat("ImpedimentIds = {0}", string.Join(",", ImpedimentIds)).AppendLine()
                .AppendFormat("CommentIds = {0}", string.Join(",", CommentIds)).AppendLine()
                .AppendFormat("IndexingProgress = {0}", IndexProgressData).AppendLine();
            return b.ToString();
        }
    }
}
