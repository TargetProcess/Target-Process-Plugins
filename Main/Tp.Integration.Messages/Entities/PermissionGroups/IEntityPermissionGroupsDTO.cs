namespace Tp.Integration.Messages.Entities.PermissionGroups
{
    public interface IEntityPermissionGroupsDTO
    {
        int EntityID { get; set; }

        string ResourceType { get; set; }

        string[] Groups { get; set; }

        string Error { get; set; }

        int? ResourceTypeId { get; set; }
    }
}
