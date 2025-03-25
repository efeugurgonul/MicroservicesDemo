using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Data;
using System.Runtime.InteropServices;

namespace ServerManagementService.Services
{
    public interface IOrganizationService
    {
        Task<bool> UserHasOrganizationAccessAsync(int userId, int organizationId);
        Task<List<int>> GetUserOrganizationIdsAsync(int userId);
        Task<int> GetDefaultOrganizationIdAsync(int userId);
        Task SetDefaultOrganizationAsync(int userId, int organizationId);
        Task AssignOrganizationToUserAsync(int userId, int organizationId);
        Task RemoveOrganizationFromUserAsync(int userId, int organizationId);
    }
    public class OrganizationService : IOrganizationService
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public OrganizationService(ServerManDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<bool> UserHasOrganizationAccessAsync(int userId, int organizationId)
        {
            var cacheKey = $"user_{userId}_org_access_{organizationId}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                return await _dbContext.UserOrganizations.AnyAsync(uo => uo.UserId == userId && uo.OrganizationId == organizationId);
            });
        }

        public async Task<List<int>> GetUserOrganizationIdsAsync(int userId)
        {
            var cacheKey = $"user_{userId}_organizations";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                return await _dbContext.UserOrganizations
                    .Where(uo => uo.UserId == userId)
                    .Select(uo => uo.OrganizationId)
                    .ToListAsync();
            });
        }

        public async Task<int> GetDefaultOrganizationIdAsync(int userId)
        {
            var cacheKey = $"user_{userId}_default_organization";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                var user = await _dbContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.DefaultOrganizationId })
                    .FirstOrDefaultAsync();

                return user?.DefaultOrganizationId ?? 0;
            });
        }
        public async Task SetDefaultOrganizationAsync(int userId, int organizationId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                //Kullanıcının bu organizasyona erişimi var mı kontrol et
                var hasAccess = await _dbContext.UserOrganizations.AnyAsync(uo => uo.UserId == userId && uo.OrganizationId == organizationId);

                if (!hasAccess)
                {
                    throw new InvalidOperationException("User does not have access to the organization");
                }

                user.DefaultOrganizationId = organizationId;
                await _dbContext.SaveChangesAsync();

                //Cache'i temizle
                _cache.Remove($"user_{userId}_default_organization");
            }
        }
        public async Task AssignOrganizationToUserAsync(int userId, int organizationId)
        {
            var existingAssignment = await _dbContext.UserOrganizations
                .AnyAsync(uo => uo.UserId == userId && uo.OrganizationId == organizationId);

            if (!existingAssignment)
            {
                _dbContext.UserOrganizations.Add(new UserOrganization
                {
                    UserId = userId,
                    OrganizationId = organizationId
                });

                await _dbContext.SaveChangesAsync();

                //Cache'i temizle
                _cache.Remove($"user_{userId}_organizations");
                _cache.Remove($"user_{userId}_org_access_{organizationId}");
            }
        }
        public async Task RemoveOrganizationFromUserAsync(int userId, int organizationId)
        {
            var existingAssignment = await _dbContext.UserOrganizations
                .FirstOrDefaultAsync(uo => uo.UserId == userId && uo.OrganizationId == organizationId);

            if (existingAssignment != null)
            {
                _dbContext.UserOrganizations.Remove(existingAssignment);
                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.Users.FindAsync(userId);
                if(user != null && user.DefaultOrganizationId == organizationId)
                {
                    //Default organizasyon kaldırıldıysa, başka default organizasyon ata
                    var newDefaultOrg = await _dbContext.UserOrganizations
                        .Where(uo => uo.UserId == userId)
                        .Select(uo => uo.OrganizationId)
                        .FirstOrDefaultAsync();

                    user.DefaultOrganizationId = newDefaultOrg; // Eğer başka bir organizasyon yoksa 0 olacak
                    await _dbContext.SaveChangesAsync();

                    //Default organizasyon cache'ini temizle
                    _cache.Remove($"user_{userId}_default_organization");
                }

                //Cache'i temizle
                _cache.Remove($"user_{userId}_organizations");
                _cache.Remove($"user_{userId}_org_access_{organizationId}");
            }
        }
    }
}
