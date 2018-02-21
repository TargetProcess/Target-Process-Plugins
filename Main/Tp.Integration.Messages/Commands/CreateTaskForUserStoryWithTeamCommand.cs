using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Messages.Commands
{
    public class CreateTaskForUserStoryWithTeamCommand : ITargetProcessCommand
    {
        public string Name { get; set; }
        public int? UserStoryID { get; set; }
        public int? OwnerID { get; set; }
    }
}
