using BetashipEcommerce.DAL.BackgroundJobs;
using BetashipEcommerce.DAL.Data;
using BetashipEcommerce.DAL.Data.Seeding;
using BetashipEcommerce.DAL.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Register IHttpContextAccessor and current user service for interceptors
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BetashipEcommerce.CORE.Repositories.ICurrentUserService, BetashipEcommerce.API.Services.CurrentUserService>();

// Add services
builder.Services.AddPersistence(builder.Configuration);
// ... other services ...

var app = builder.Build();

// Seed database on startup (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// Configure Hangfire dashboard with authorization
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireDashboardAuthorizationFilter(app.Environment.IsDevelopment())
    },
    DashboardTitle = "BetaShip Ecommerce - Background Jobs",
    StatsPollingInterval = 5000 // Refresh stats every 5 seconds
});

// Configure recurring jobs (including cart sync jobs)
app.Services.ConfigureRecurringJobs();

app.MapControllers();

app.Run();
