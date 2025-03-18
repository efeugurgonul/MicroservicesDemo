using Microsoft.EntityFrameworkCore;
using ServerManagentService.Domain.Entities;

namespace ServerManagentService.Data
{
    public class ServerManDbContext(DbContextOptions<ServerManDbContext> options) : DbContext(options)
    {
        public DbSet<Organization> Organizations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
