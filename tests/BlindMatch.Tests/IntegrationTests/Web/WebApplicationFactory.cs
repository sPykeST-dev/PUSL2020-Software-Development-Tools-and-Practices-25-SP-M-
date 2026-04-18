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

public class BlindMatchWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; } = "BlindMatchFunctional_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            var configType = typeof(IDbContextOptionsConfiguration<ApplicationDbContext>);
            var configDescriptors = services.Where(d => d.ServiceType == configType).ToList();
            foreach (var d in configDescriptors) services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseInMemoryDatabase(DatabaseName)
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme    = "Test";
            });
        });
    }

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

    public HttpClient CreateAnonClient(bool allowAutoRedirect = false) =>
        CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = allowAutoRedirect,
            HandleCookies     = true
        });

    public ApplicationDbContext GetSeedContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public static string ExtractAntiForgeryToken(string html)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""");
        var m = regex.Match(html);
        if (!m.Success)
            throw new InvalidOperationException(
                "Anti-forgery token not found in HTML. Is the page rendering correctly?");
        return m.Groups[1].Value;
    }
}

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
