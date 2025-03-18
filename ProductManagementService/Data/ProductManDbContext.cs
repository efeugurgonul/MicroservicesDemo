using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductManagementService.Domain.Entities;

namespace ProductManagementService.Data
{
    public class ProductManDbContext : DbContext
    {
        public ProductManDbContext(DbContextOptions<ProductManDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            base.OnModelCreating(modelBuilder);
        }
    }
}
