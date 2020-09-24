namespace Tp.Integration.Messages.Entities.PermissionGroups
{
    public interface IUserPermissionGroupsDTO
    {
        int UserId { get; set; }

        string[] Groups { get; set; }

        string Error { get; set; }
    }
}
