using Common.Core.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Data;

namespace ServerManagementService.Services
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(int userId, ActionType action, ResourceType resource);
        Task<bool> UserHasPermissionAsync(int userId, Permission permission);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task AssignPermissionToUserAsync(int userId, Permission permission);
        Task RemovePermissionFromUserAsync(int userId, Permission permission);
    }
    public class PermissionService : IPermissionService
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IMemoryCache _cache;
        public PermissionService(ServerManDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<bool> UserHasPermissionAsync(int userId, ActionType action, ResourceType resource)
        {
            // PermissionHelper ile ActionType ve ResourceType'a göre ilgili Permission'ı bul
            var permission = PermissionHelper.GetPermission(action, resource);
            if (permission == null)
            {
                return false;
            }

            return await UserHasPermissionAsync(userId, permission.Value);
        }

        public async Task<bool> UserHasPermissionAsync(int userId, Permission permission)
        {
            var cacheKey = $"user_{userId}_permission_{permission}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                // Direkt enum değerini kontrol et
                bool hasPermission = await _dbContext.UserPermissions.AnyAsync(up => up.UserId == userId && up.Permission == permission);

                // Manage yetkisi de aranabilir
                if (!hasPermission)
                {
                    var resourceType = PermissionHelper.GetResourceType(permission);
                    var managePermission = PermissionHelper.GetPermission(ActionType.Manage, resourceType);

                    if (managePermission.HasValue)
                    {
                        hasPermission = await _dbContext.UserPermissions.AnyAsync(up => up.UserId == userId && up.Permission == managePermission.Value);
                    }
                }

                return hasPermission;
            });
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            var cacheKey = $"user_{userId}_permissions";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                return await _dbContext.UserPermissions.Where(up => up.UserId == userId).Select(up => up.Permission).ToListAsync();
            });
        }

        public async Task AssignPermissionToUserAsync(int userId, Permission permission)
        {
            var existingPermission = _dbContext.UserPermissions.FirstOrDefault(up => up.UserId == userId && up.Permission == permission);

            if (existingPermission == null)
            {
                _dbContext.UserPermissions.Add(new UserPermission
                {
                    UserId = userId,
                    Permission = permission
                });

                await _dbContext.SaveChangesAsync();

                //Cache'i temizle
                InvalidateUserPermissionsCache(userId);
            }
        }

        public async Task RemovePermissionFromUserAsync(int userId, Permission permission)
        {
            var existingPermission = _dbContext.UserPermissions.FirstOrDefault(up => up.UserId == userId && up.Permission == permission);

            if (existingPermission != null)
            {
                _dbContext.UserPermissions.Remove(existingPermission);
                await _dbContext.SaveChangesAsync();

                //Cache'i temizle
                InvalidateUserPermissionsCache(userId);
            }
        }

        private void InvalidateUserPermissionsCache(int userId)
        {
            var cacheKey = $"user_{userId}_permissions";
            _cache.Remove(cacheKey);

            // Tek tek permission cache'lerini temizlemek için patterns kullan
            var cacheKeys = _cache.GetKeys()
                                  .Where(k => k.StartsWith($"user_{userId}_permission_"))
                                  .ToList();

            foreach (var key in cacheKeys)
            {
                _cache.Remove(key);
            }
        }
    }

    // Extension method to get cache keys
    public static class MemoryCacheExtensions
    {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = cache =>
            cache.GetType().GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cache);

        public static IEnumerable<string> GetKeys(this IMemoryCache cache)
        {
            var memoryCache = cache as MemoryCache;
            if (memoryCache == null) return Enumerable.Empty<string>();

            var entriesCollection = GetEntriesCollection(memoryCache);
            if (entriesCollection == null) return Enumerable.Empty<string>();

            return entriesCollection.GetType()
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .First(p => p.Name == "Keys")
                .GetValue(entriesCollection) as IEnumerable<string> ?? Enumerable.Empty<string>();
        }
    }
}
