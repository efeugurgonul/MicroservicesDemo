namespace Common.Core.Auth
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public Permission Permission { get; set; }
        public string Name => Permission.ToString();
    }

    public class PermissionCheckRequest
    {
        public int UserId { get; set; }
        public ResourceType ResourceType { get; set; }
        public ActionType Action { get; set; }
    }

    public class PermissionCheckResponse
    {
        public bool HasPermission { get; set; }
    }

    public class OrganizationCheckRequest
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class OrganizationCheckResponse
    {
        public bool HasAccess { get; set; }
    }
}
