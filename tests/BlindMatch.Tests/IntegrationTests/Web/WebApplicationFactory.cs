using System.Security.Claims;
using System.Text.Encodings.Web;
using BlindMatch.Core.Entities;
using BlindMatch.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlindMatch.Tests.IntegrationTests.Web;

/// <summary>
/// Custom <see cref="WebApplicationFactory{TProgram}"/> that replaces the SQL Server
/// database with an EF Core InMemory store and swaps Identity cookie auth for a
/// simple header-based fake auth scheme (X-Test-UserId / X-Test-UserRole).
/// </summary>
public class BlindMatchWebApplicationFactory : WebApplicationFactory<Program>
{
    // Shared database name so all clients in the same test class see the same data.
    public string DatabaseName { get; } = "BlindMatchFunctional_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ── Replace real DbContext with InMemory ─────────────────────────
            // Remove DbContextOptions<T> registered by AddInfrastructure
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            // Also remove all IDbContextOptionsConfiguration<T> entries — these hold
            // the SQL Server provider configuration added by AddInfrastructure.
            // Without removing them, both SqlServer and InMemory providers end up
            // registered in the same EF Core internal service provider, which throws.
            var configType = typeof(IDbContextOptionsConfiguration<ApplicationDbContext>);
            var configDescriptors = services.Where(d => d.ServiceType == configType).ToList();
            foreach (var d in configDescriptors) services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseInMemoryDatabase(DatabaseName)
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            // ── Register fake auth scheme ────────────────────────────────────
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            // Override Identity's default schemes so our handler runs first.
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme    = "Test";
            });
        });
    }

    // ── Helper factories ─────────────────────────────────────────────────────

    /// <summary>Creates a client that sends the given userId and role on every request.</summary>
    public HttpClient CreateAuthenticatedClient(string userId, string role,
        bool allowAutoRedirect = false)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = allowAutoRedirect,
            HandleCookies     = true
        });
        client.DefaultRequestHeaders.Add("X-Test-UserId",   userId);
        client.DefaultRequestHeaders.Add("X-Test-UserRole", role);
        return client;
    }

    /// <summary>Creates an anonymous client (no auth headers).</summary>
    public HttpClient CreateAnonClient(bool allowAutoRedirect = false) =>
        CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = allowAutoRedirect,
            HandleCookies     = true
        });

    /// <summary>Seeds data directly into the shared InMemory database.</summary>
    public ApplicationDbContext GetSeedContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    /// <summary>
    /// Extracts the ASP.NET Core anti-forgery token from a page's HTML body.
    /// </summary>
    public static string ExtractAntiForgeryToken(string html)
    {
        // AntiForgeryToken renders: name="__RequestVerificationToken" type="hidden" value="..."
        // The regex must tolerate attributes (e.g. type="hidden") between name and value.
        var regex = new System.Text.RegularExpressions.Regex(
            @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""");
        var m = regex.Match(html);
        if (!m.Success)
            throw new InvalidOperationException(
                "Anti-forgery token not found in HTML. Is the page rendering correctly?");
        return m.Groups[1].Value;
    }
}

// ── Fake authentication handler ───────────────────────────────────────────────

/// <summary>
/// Reads <c>X-Test-UserId</c> and <c>X-Test-UserRole</c> request headers and
/// builds a <see cref="ClaimsPrincipal"/> without touching ASP.NET Core Identity.
/// </summary>
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-UserId",   out var userId)   ||
            !Request.Headers.TryGetValue("X-Test-UserRole", out var role))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name,           userId.ToString()),
            new Claim(ClaimTypes.Role,           role.ToString())
        };

        var identity  = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket    = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
