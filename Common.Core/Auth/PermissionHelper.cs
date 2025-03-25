using System;
using System.Collections.Generic;
using System.Linq;
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
            
            //// License permissions
            //{ Permission.ViewLicenses, (ActionType.Read, ResourceType.License) },
            //{ Permission.CreateLicense, (ActionType.Create, ResourceType.License) },
            //{ Permission.UpdateLicense, (ActionType.Update, ResourceType.License) },
            //{ Permission.DeleteLicense, (ActionType.Delete, ResourceType.License) },
            
            //// Parameter permissions
            //{ Permission.ViewParameters, (ActionType.Read, ResourceType.Parameter) },
            //{ Permission.UpdateParameters, (ActionType.Update, ResourceType.Parameter) },
            
            //// Schedule permissions
            //{ Permission.ViewSchedules, (ActionType.Read, ResourceType.Schedule) },
            //{ Permission.CreateSchedule, (ActionType.Create, ResourceType.Schedule) },
            //{ Permission.UpdateSchedule, (ActionType.Update, ResourceType.Schedule) },
            //{ Permission.DeleteSchedule, (ActionType.Delete, ResourceType.Schedule) },
            
            //// Term permissions
            //{ Permission.ViewTerms, (ActionType.Read, ResourceType.Term) },
            //{ Permission.CreateTerm, (ActionType.Create, ResourceType.Term) },
            //{ Permission.UpdateTerm, (ActionType.Update, ResourceType.Term) },
            //{ Permission.DeleteTerm, (ActionType.Delete, ResourceType.Term) }
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
    }
}
