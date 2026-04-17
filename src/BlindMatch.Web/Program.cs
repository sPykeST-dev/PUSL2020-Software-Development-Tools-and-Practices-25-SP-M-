using BlindMatch.Core.Common;
using BlindMatch.Core.Entities;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Infrastructure.DependencyInjection;
using BlindMatch.Infrastructure.Identity;
using BlindMatch.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.StudentOnly,      p => p.RequireRole(Roles.Student));
    options.AddPolicy(Policies.SupervisorOnly,   p => p.RequireRole(Roles.Supervisor));
    options.AddPolicy(Policies.ModuleLeaderOnly, p => p.RequireRole(Roles.ModuleLeader));
    options.AddPolicy(Policies.AdminOnly,        p => p.RequireRole(Roles.SystemAdministrator));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<ActiveUserFilter>();
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Migrate database and seed roles on startup.
// Retries handle the window between SQL Server accepting TCP connections
// and being fully ready to run queries (common in Docker).
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger   = services.GetRequiredService<ILogger<Program>>();

    for (int attempt = 1; attempt <= 10; attempt++)
    {
        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleSeeder.SeedRolesAsync(roleManager);

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            await UserSeeder.SeedAdminAsync(userManager);

            if (app.Configuration.GetValue<bool>("SEED_DEMO_DATA"))
                await DemoDataSeeder.SeedAsync(services);

            logger.LogInformation("Database ready.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Database not ready (attempt {Attempt}/10): {Message}", attempt, ex.Message);
            if (attempt == 10) throw;
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();

public partial class Program { }
