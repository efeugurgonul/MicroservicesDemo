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
        License,
        Parameter,
        Schedule,
        Term,
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

        // License permissions
        ViewLicenses,
        CreateLicense,
        UpdateLicense,
        DeleteLicense,

        // Parameter permissions
        ViewParameters,
        UpdateParameters,

        // Schedule permissions
        ViewSchedules,
        CreateSchedule,
        UpdateSchedule,
        DeleteSchedule,

        // Term permissions
        ViewTerms,
        CreateTerm,
        UpdateTerm,
        DeleteTerm
    }
}
