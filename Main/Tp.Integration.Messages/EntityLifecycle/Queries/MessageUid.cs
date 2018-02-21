using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class RetrieveAllMessageUidsQuery : QueryBase
    {
        public string MailServer { get; set; }
        public string MailLogin { get; set; }

        public override DtoType DtoType
        {
            get { return new DtoType(typeof(MessageUidDTO)); }
        }
    }

    [Serializable]
    public class MessageUidQueryResult : QueryResult<MessageUidDTO>, ISagaMessage
    {
    }
}
