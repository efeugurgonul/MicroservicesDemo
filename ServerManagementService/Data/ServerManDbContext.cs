using Microsoft.EntityFrameworkCore;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Domain.Entities;

namespace ServerManagementService.Data
{
    public class ServerManDbContext(DbContextOptions<ServerManDbContext> options) : DbContext(options)
    {
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserPermission> UserPermissions { get; set; } = null!;
        public DbSet<UserOrganization> UserOrganizations { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Permission enum için dönüşüm yapılandırması
            modelBuilder.Entity<UserPermission>()
                .Property(e => e.Permission)
                .HasConversion<string>();

            // İndeksler
            modelBuilder.Entity<UserPermission>()
                .HasIndex(up => new { up.UserId, up.Permission })
                .HasDatabaseName("IX_UserPermission_UserId_Permission");

            modelBuilder.Entity<UserOrganization>()
                .HasIndex(uo => new { uo.UserId, uo.OrganizationId })
                .HasDatabaseName("IX_UserOrganization_UserId_OrganizationId");

            // İlişkiler
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserOrganization>()
                .HasOne(uo => uo.User)
                .WithMany(u => u.Organizations)
                .HasForeignKey(uo => uo.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserOrganization>()
                .HasOne(uo => uo.Organization)
                .WithMany()
                .HasForeignKey(uo => uo.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
