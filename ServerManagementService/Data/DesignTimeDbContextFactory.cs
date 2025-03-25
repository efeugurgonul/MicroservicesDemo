using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServerManagementService.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ServerManDbContext>
    {
        public ServerManDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ServerManDbContext>();
            var connectionString = configuration.GetConnectionString("ServerManagementConnection");

            builder.UseNpgsql(connectionString);

            return new ServerManDbContext(builder.Options);
        }
    }
}