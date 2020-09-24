using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
    [Serializable]
    public class CreateCommand : ITargetProcessCommand
    {
        public DataTransferObject Dto { get; set; }

        public override string ToString()
        {
            return base.ToString() + ":" + Dto.ToString();
        }
    }

    [Serializable]
    public class UpdateCommand : ITargetProcessCommand
    {
        public UpdateCommand()
        {
            ChangedFields = Array.Empty<string>();
        }

        public DataTransferObject Dto { get; set; }
        public string[] ChangedFields { get; set; }

        public override string ToString()
        {
            return "{0}:{1}".Fmt(base.ToString(), Dto.ToString());
        }
    }

    [Serializable]
    public class DeleteCommand : ITargetProcessCommand
    {
        public int Id { get; set; }
        public DtoType DtoType { get; set; }

        public override string ToString()
        {
            return "{0}:{1}({2})".Fmt(base.ToString(), DtoType.ToString(), Id);
        }
    }
}
