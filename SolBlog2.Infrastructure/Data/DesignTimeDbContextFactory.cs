using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.IO;

namespace SolBlog2.Infrastructure.Data
{
    // Don't seal unless you really want to; EF doesn't care.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration so the factory uses the same settings as runtime
            var webDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SolBlog2.Web"));

            var cfg = new ConfigurationBuilder()
                .SetBasePath(webDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = cfg.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection not found.");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(cs, npgsql =>
                {
                    // Point migrations at the project that contains them (usually Infrastructure)
                    npgsql.MigrationsAssembly("SolBlog2.Infrastructure");
                });

            return new ApplicationDbContext(builder.Options);
        }
    }
}
