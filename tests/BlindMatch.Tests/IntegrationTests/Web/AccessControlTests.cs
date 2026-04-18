using System.Net;
using BlindMatch.Core.Entities;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Tests.TestHelpers.EntityBuilders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BlindMatch.Tests.IntegrationTests.Web;

public class AccessControlTests : IClassFixture<BlindMatchWebApplicationFactory>
{
    private readonly HttpClient _anonClient;
    private readonly HttpClient _studentClient;
    private readonly HttpClient _supervisorClient;
    private readonly HttpClient _adminClient;
    private readonly HttpClient _moduleLeaderClient;

    public AccessControlTests(BlindMatchWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ctx.Database.EnsureCreated();
        SeedTestUsers(ctx);

        var opts = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };

        _anonClient         = factory.CreateClient(opts);
        _studentClient      = factory.CreateAuthenticatedClient("stu-ac-test",   "Student",             allowAutoRedirect: false);
        _supervisorClient   = factory.CreateAuthenticatedClient("sup-ac-test",   "Supervisor",          allowAutoRedirect: false);
        _adminClient        = factory.CreateAuthenticatedClient("admin-ac-test", "SystemAdministrator", allowAutoRedirect: false);
        _moduleLeaderClient = factory.CreateAuthenticatedClient("ml-ac-test",    "ModuleLeader",        allowAutoRedirect: false);
    }

    private static void SeedTestUsers(ApplicationDbContext ctx)
    {
        if (ctx.Users.Any(u => u.Id == "stu-ac-test")) return;

        ctx.Students.Add(new StudentBuilder().WithId("stu-ac-test").WithEmail("stu-ac@test.com").Build());
        ctx.Supervisors.Add(new SupervisorBuilder().WithId("sup-ac-test").WithEmail("sup-ac@test.com").Build());

        ctx.Users.AddRange(
            new ApplicationUser
            {
                Id                 = "admin-ac-test",
                UserName           = "admin-ac@test.com",
                Email              = "admin-ac@test.com",
                NormalizedEmail    = "ADMIN-AC@TEST.COM",
                NormalizedUserName = "ADMIN-AC@TEST.COM",
                FirstName          = "Test",
                LastName           = "Admin",
                IsActive           = true,
                SecurityStamp      = Guid.NewGuid().ToString()
            },
            new ApplicationUser
            {
                Id                 = "ml-ac-test",
                UserName           = "ml-ac@test.com",
                Email              = "ml-ac@test.com",
                NormalizedEmail    = "ML-AC@TEST.COM",
                NormalizedUserName = "ML-AC@TEST.COM",
                FirstName          = "Test",
                LastName           = "ModuleLeader",
                IsActive           = true,
                SecurityStamp      = Guid.NewGuid().ToString()
            });

        ctx.SaveChanges();
    }

    [Fact]
    public async Task Login_Anonymous_Returns200()
    {
        var resp = await _anonClient.GetAsync("/Account/Login");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProposalCreate_Anonymous_Returns401()
    {
        var resp = await _anonClient.GetAsync("/Proposal/Create");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProposalCreate_Supervisor_Returns403()
    {
        var resp = await _supervisorClient.GetAsync("/Proposal/Create");

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProposalCreate_Student_DoesNotReturn401Or403()
    {
        var resp = await _studentClient.GetAsync("/Proposal/Create");

        resp.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
            "student should not receive a challenge");
        resp.StatusCode.Should().NotBe(HttpStatusCode.Forbidden,
            "student should not be forbidden");
    }

    [Fact]
    public async Task UserManagement_Anonymous_Returns401()
    {
        var resp = await _anonClient.GetAsync("/UserManagement/Index");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserManagement_Student_Returns403()
    {
        var resp = await _studentClient.GetAsync("/UserManagement/Index");

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UserManagement_Admin_Returns200()
    {
        var resp = await _adminClient.GetAsync("/UserManagement/Index");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AuditLog_Anonymous_Returns401()
    {
        var resp = await _anonClient.GetAsync("/admin/audit-log");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuditLog_Supervisor_Returns403()
    {
        var resp = await _supervisorClient.GetAsync("/admin/audit-log");

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Dashboard_Anonymous_Returns401()
    {
        var resp = await _anonClient.GetAsync("/module-leader/dashboard");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Dashboard_ModuleLeader_Returns200()
    {
        var resp = await _moduleLeaderClient.GetAsync("/module-leader/dashboard");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BrowseProposals_Anonymous_Returns401()
    {
        var resp = await _anonClient.GetAsync("/SupervisorBrowse/Index");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BrowseProposals_Student_Returns403()
    {
        var resp = await _studentClient.GetAsync("/SupervisorBrowse/Index");

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
