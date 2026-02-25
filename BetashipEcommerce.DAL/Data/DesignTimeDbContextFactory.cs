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

            // Prefer explicit environment variable, fall back to local PostgreSQL for developer convenience
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                ?? "User Id=postgres.oquxoyqxnbniqistwlrl;Password=Kodemon100000$;Server=aws-1-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres";

            builder.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            return new ApplicationDbContext(builder.Options);
        }
    }
}

