using BetashipEcommerce.API.Configuration;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.API.Filters;
using BetashipEcommerce.API.Middleware;
using BetashipEcommerce.API.Services;
using BetashipEcommerce.APP;
using BetashipEcommerce.DAL.BackgroundJobs;
using BetashipEcommerce.DAL.Data.Seeding;
using BetashipEcommerce.DAL.Persistence;
using Hangfire;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────────────────────────────────
// Infrastructure & cross-cutting services
// ──────────────────────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// Register the single CurrentUserService for both ICurrentUserService contracts:
//   - CORE version  → used by DAL interceptors (audit columns, soft-delete)
//   - APP version   → used by MediatR pipeline behaviours (Auditing, Authorization)
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<BetashipEcommerce.CORE.Repositories.ICurrentUserService>(
    sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<BetashipEcommerce.APP.Common.Interfaces.ICurrentUserService>(
    sp => sp.GetRequiredService<CurrentUserService>());

// ──────────────────────────────────────────────────────────────────────────────
// Domain + Application + Persistence layers
// ──────────────────────────────────────────────────────────────────────────────
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

// ──────────────────────────────────────────────────────────────────────────────
// API-layer services: Swagger, CORS, JWT auth, rate limiting, auth policies
// ──────────────────────────────────────────────────────────────────────────────
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddScoped<IAuthService, SupabaseAuthService>();

// ──────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ──────────────────────────────────────────────────────────────────────────────
// Database seeding (Development only)
// ──────────────────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    try
    {
        await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
        app.Logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database seeding failed");
    }
}

// ──────────────────────────────────────────────────────────────────────────────
// Middleware pipeline (order matters)
// ──────────────────────────────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<SupabaseApiKeyMiddleware>();

app.UseHttpsRedirection();
app.UseCorsPolicy();
app.UseApiRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

// ──────────────────────────────────────────────────────────────────────────────
// Swagger UI (Development only)
// ──────────────────────────────────────────────────────────────────────────────
app.UseSwaggerUi();

// ──────────────────────────────────────────────────────────────────────────────
// Hangfire dashboard
// ──────────────────────────────────────────────────────────────────────────────
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireAuthorizationFilter(app.Environment.IsDevelopment())
    },
    DashboardTitle       = "BetaShip Ecommerce - Background Jobs",
    StatsPollingInterval = 5000
});

// ──────────────────────────────────────────────────────────────────────────────
// Recurring background jobs
// ──────────────────────────────────────────────────────────────────────────────
_ = Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
{
    try
    {
        app.Services.ConfigureRecurringJobs();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to configure recurring jobs");
    }
});

// ──────────────────────────────────────────────────────────────────────────────
// Minimal API endpoint discovery
// ──────────────────────────────────────────────────────────────────────────────
app.MapEndpoints(Assembly.GetExecutingAssembly());

app.Run();
