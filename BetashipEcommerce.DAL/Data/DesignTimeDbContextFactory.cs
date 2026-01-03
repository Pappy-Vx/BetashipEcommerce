using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace BetashipEcommerce.DAL.Data
{
    /// <summary>
    /// Provides a design-time factory for ApplicationDbContext to be used by EF tools.
    /// This avoids needing the full application DI (and runtime-only services) when running migrations.
    /// </summary>
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Prefer explicit environment variable, fall back to LocalDB for developer convenience
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=BetashipEcommerce;Trusted_Connection=True;";

            builder.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            return new ApplicationDbContext(builder.Options);
        }
    }
}
