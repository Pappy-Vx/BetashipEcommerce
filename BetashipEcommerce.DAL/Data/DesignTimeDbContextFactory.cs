using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BetashipEcommerce.DAL.Data
{
    /// <summary>
    /// Provides a design-time factory for ApplicationDbContext to be used by EF tools.
    /// This avoids needing the full application DI (and runtime-only services) when running migrations.
    /// 
    /// Connection string resolution order:
    ///   1. Environment variable "DefaultConnection"
    ///   2. .NET User Secrets (via UserSecretsId in API project)
    ///   3. appsettings.Development.json
    ///   4. appsettings.json
    /// 
    /// ⚠️ NEVER hardcode credentials here — use User Secrets or environment variables.
    /// </summary>
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration from the API project's settings + User Secrets
            var apiProjectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "BetashipEcommerce.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(apiProjectPath, "appsettings.json"), optional: true)
                .AddJsonFile(Path.Combine(apiProjectPath, "appsettings.Development.json"), optional: true)
                .AddUserSecrets("bc2654d9-3c9c-4cc8-a181-db9be8ed99d5", reloadOnChange: false) // API project's UserSecretsId
                .AddEnvironmentVariables()
                .Build();

            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found. " +
                    "Set it via: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"your-connection-string\" " +
                    "--project ../BetashipEcommerce.API, or set the 'DefaultConnection' environment variable.");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            return new ApplicationDbContext(builder.Options);
        }
    }
}
