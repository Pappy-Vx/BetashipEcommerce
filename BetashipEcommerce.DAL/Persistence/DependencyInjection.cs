using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.UnitOfWork;
using BetashipEcommerce.DAL.BackgroundJobs;
using BetashipEcommerce.DAL.Data;
using BetashipEcommerce.DAL.Data.Seeding;
using BetashipEcommerce.DAL.Interceptors;
using BetashipEcommerce.DAL.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add DbContext with Npgsql (PostgreSQL) for Supabase
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                });

                // Add interceptors
                options.AddInterceptors(
                    sp.GetRequiredService<AuditableEntityInterceptor>(),
                    sp.GetRequiredService<DomainEventInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>());

                // Enable detailed errors in development
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });

            // Register interceptors
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<DomainEventInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<DatabaseSeeder>();

            // ─────────────────────────────────────────────────────────
            // Redis Configuration (Hybrid Cart Strategy)
            // ─────────────────────────────────────────────────────────
            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? "localhost:6379"; // Default to local Redis

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnectionString);
                configOptions.AbortOnConnectFail = false; // Don't crash if Redis is down
                configOptions.ConnectRetry = 5;
                configOptions.ConnectTimeout = 5000;
                configOptions.SyncTimeout = 3000;
                configOptions.AsyncTimeout = 3000;

                var logger = sp.GetRequiredService<ILogger<ConnectionMultiplexer>>();
                logger.LogInformation("🔌 Connecting to Redis: {RedisConnection}", redisConnectionString);

                return ConnectionMultiplexer.Connect(configOptions);
            });

            // Register Redis Cart Cache Service
            services.AddScoped<ICartCacheService, CartCacheService>();

            // Register Cart Sync Background Job
            services.AddTransient<CartSyncBackgroundJob>();

            // ─────────────────────────────────────────────────────────
            // Hangfire Configuration (Background Jobs)
            // ─────────────────────────────────────────────────────────
            // Add Hangfire for background jobs (PostgreSQL storage for Supabase)
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(connectionString)));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 5;
                options.Queues = new[] { "default", "outbox", "notifications", "maintenance" };
            });

            return services;
        }
    }
}

