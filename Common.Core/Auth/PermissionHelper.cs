using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.Auth
{
    public static class PermissionHelper
    {
        // Permission ile ActionType ve ResourceType arasındaki eşleştirmeyi sağlar
        private static readonly Dictionary<Permission, (ActionType Action, ResourceType Resource)> _permissionMap = new()
        {
            // User permissions
            { Permission.ViewUsers, (ActionType.Read, ResourceType.User) },
            { Permission.CreateUser, (ActionType.Create, ResourceType.User) },
            { Permission.UpdateUser, (ActionType.Update, ResourceType.User) },
            { Permission.DeleteUser, (ActionType.Delete, ResourceType.User) },
            { Permission.ManageUserPermissions, (ActionType.Manage, ResourceType.User) },
            
            // Organization permissions
            { Permission.ViewOrganizations, (ActionType.Read, ResourceType.Organization) },
            { Permission.CreateOrganization, (ActionType.Create, ResourceType.Organization) },
            { Permission.UpdateOrganization, (ActionType.Update, ResourceType.Organization) },
            { Permission.DeleteOrganization, (ActionType.Delete, ResourceType.Organization) },
            { Permission.ManageOrganizationUsers, (ActionType.Manage, ResourceType.Organization) },
            
            // Product permissions
            { Permission.ViewProducts, (ActionType.Read, ResourceType.Product) },
            { Permission.CreateProduct, (ActionType.Create, ResourceType.Product) },
            { Permission.UpdateProduct, (ActionType.Update, ResourceType.Product) },
            { Permission.DeleteProduct, (ActionType.Delete, ResourceType.Product) },
        };

        // ActionType ve ResourceType'a göre Permission almak için
        public static Permission? GetPermission(ActionType action, ResourceType resource)
        {
            return _permissionMap
                .Where(x => x.Value.Action == action && x.Value.Resource == resource)
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        // Permission'a göre ActionType almak için
        public static ActionType GetActionType(Permission permission)
        {
            return _permissionMap.TryGetValue(permission, out var value) ? value.Action : ActionType.Read;
        }

        // Permission'a göre ResourceType almak için
        public static ResourceType GetResourceType(Permission permission)
        {
            return _permissionMap.TryGetValue(permission, out var value) ? value.Resource : ResourceType.User;
        }

        // String'den Permission dönüşümü
        public static Permission? GetPermissionFromString(string permissionString)
        {
            if (Enum.TryParse<Permission>(permissionString, true, out var permission))
                return permission;

            return null;
        }

        // Bir kullanıcının izinleri arasında belirli bir iznin olup olmadığını kontrol eder
        public static bool HasPermission(IEnumerable<Permission> userPermissions, Permission requiredPermission)
        {
            return userPermissions.Contains(requiredPermission);
        }

        // Bir kullanıcının izinleri arasında belirli bir kaynak üzerinde belirli bir eylemi yapma izninin olup olmadığını kontrol eder
        public static bool HasActionOnResource(IEnumerable<Permission> userPermissions, ActionType action, ResourceType resource)
        {
            // İlgili özel izni bul
            var requiredPermission = GetPermission(action, resource);

            // İzin bulunamadıysa false dön
            if (requiredPermission == null)
                return false;

            // Spesifik izni kontrol et
            bool hasSpecificPermission = userPermissions.Contains(requiredPermission.Value);

            // Yönetim iznini kontrol et
            var managePermission = GetPermission(ActionType.Manage, resource);
            bool hasManagePermission = managePermission.HasValue && userPermissions.Contains(managePermission.Value);

            return hasSpecificPermission || hasManagePermission;
        }


        public static bool CheckPermissionFromClaims(IEnumerable<Claim> claims, ResourceType resourceType, ActionType action)
        {
            try
            {
                // Tokendan izinleri al
                var permissions = claims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToList();

                // Gerekli izni belirle
                var requiredPermission = GetPermission(action, resourceType);
                if (requiredPermission == null)
                    return false;

                // Spesifik izni kontrol et
                string requiredPermissionStr = requiredPermission.ToString();
                bool hasSpecificPermission = permissions.Contains(requiredPermissionStr);

                // Yönetim iznini kontrol et
                var managePermission = GetPermission(ActionType.Manage, resourceType);
                bool hasManagePermission = false;
                if (managePermission.HasValue)
                {
                    string managePermissionStr = managePermission.Value.ToString();
                    hasManagePermission = permissions.Contains(managePermissionStr);
                }

                return hasSpecificPermission || hasManagePermission;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckOrganizationAccessFromClaims(IEnumerable<Claim> claims, int organizationId)
        {
            // Default organizasyonu kontrol et
            var defaultOrgClaim = claims.FirstOrDefault(c => c.Type == "defaultOrganizationId");
            if (defaultOrgClaim != null && int.TryParse(defaultOrgClaim.Value, out int defaultOrgId))
            {
                if (defaultOrgId == organizationId)
                    return true;
            }

            // Organizasyon erişim listesini kontrol et
            var orgAccessClaims = claims.Where(c => c.Type == "orgAccess").Select(c => c.Value);
            foreach (var orgClaim in orgAccessClaims)
            {
                if (int.TryParse(orgClaim, out int orgId) && orgId == organizationId)
                    return true;
            }

            // Admin kullanıcıları için her organizasyona erişim izni ver
            var isAdmin = claims
                .Where(c => c.Type == "permission")
                .Any(c => c.Value == "ManageOrganizationUsers" || c.Value == "ManageUserPermissions");

            return isAdmin;
        }
    }
}
