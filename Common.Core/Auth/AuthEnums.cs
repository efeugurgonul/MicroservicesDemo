namespace Common.Core.Auth
{
    public enum ActionType
    {
        Create,
        Read,
        Update,
        Delete,
        Manage
    }

    public enum ResourceType
    {
        User,
        Organization,
        Product,        
        Permission
    }

    public enum Permission
    {
        // User permissions
        ViewUsers,
        CreateUser,
        UpdateUser,
        DeleteUser,
        ManageUserPermissions,

        // Organization permissions
        ViewOrganizations,
        CreateOrganization,
        UpdateOrganization,
        DeleteOrganization,
        ManageOrganizationUsers,

        // Product permissions
        ViewProducts,
        CreateProduct,
        UpdateProduct,
        DeleteProduct,
    }
}
