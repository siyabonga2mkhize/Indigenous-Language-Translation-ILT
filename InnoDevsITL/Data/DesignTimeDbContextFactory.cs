using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InnoDevsITL.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InnoDbContext>
    {
        public InnoDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("Default");

            // Build DbContext options
            var builder = new DbContextOptionsBuilder<InnoDbContext>();
            builder.UseSqlServer(connectionString);

            return new InnoDbContext(builder.Options);
        }
    }
}