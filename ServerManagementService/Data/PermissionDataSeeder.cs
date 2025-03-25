using Common.Core.Auth;
using Microsoft.EntityFrameworkCore;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Services;
using ServerManagementService.Data;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Data
{
    public class PermissionDataSeeder
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public PermissionDataSeeder(ServerManDbContext dbContext, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            // Veritabanı oluşturuldu mu kontrol et
            await _dbContext.Database.MigrateAsync();

            // Admin kullanıcı ve varsayılan organizasyon
            await SeedDefaultOrganization();
            await SeedAdminUser();
        }

        private async Task SeedDefaultOrganization()
        {
            if (!await _dbContext.Organizations.AnyAsync())
            {
                var defaultOrg = new Organization
                {
                    Name = "Default Organization",
                    Description = "System default organization",
                    ActiveStatus = 1
                };

                _dbContext.Organizations.Add(defaultOrg);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task SeedAdminUser()
        {
            if (!await _dbContext.Users.AnyAsync(u => u.Username == "admin"))
            {
                var defaultOrgId = await _dbContext.Organizations
                    .Where(o => o.Name == "Default Organization")
                    .Select(o => o.Id)
                    .FirstOrDefaultAsync();

                if (defaultOrgId == 0)
                    return;

                // Admin kullanıcısını oluştur
                _passwordService.CreatePasswordHash("Admin123!", out byte[] passwordHash, out byte[] passwordSalt);

                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    DefaultOrganizationId = defaultOrgId,
                    IsActive = true
                };

                _dbContext.Users.Add(adminUser);
                await _dbContext.SaveChangesAsync();

                // Admin kullanıcısına organizasyon erişimi ver
                _dbContext.UserOrganizations.Add(new UserOrganization
                {
                    UserId = adminUser.Id,
                    OrganizationId = defaultOrgId
                });

                // Tüm izinleri admin kullanıcısına ver
                foreach (Permission permission in Enum.GetValues(typeof(Permission)))
                {
                    _dbContext.UserPermissions.Add(new UserPermission
                    {
                        UserId = adminUser.Id,
                        Permission = permission
                    });
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
