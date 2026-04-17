using System.Net;
using BlindMatch.Core.Enums;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Tests.TestHelpers.EntityBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace BlindMatch.Tests.IntegrationTests.Web;

/// <summary>
/// Full end-to-end HTTP tests that exercise the blind-match flow using an
/// InMemory database and a fake authentication handler.
/// </summary>
public class BlindMatchFlowTests : IClassFixture<BlindMatchWebApplicationFactory>
{
    private readonly BlindMatchWebApplicationFactory _factory;

    public BlindMatchFlowTests(BlindMatchWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // ── TC-FLOW-01: Full confirm-interest flow via HTTP ───────────────────────

    [Fact]
    public async Task ConfirmInterest_ValidFlow_CreatesMatchAndReveal()
    {
        // ── Arrange: seed data directly into the InMemory DB ─────────────────
        using var seedScope = _factory.Services.CreateScope();
        var ctx = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ctx.Database.EnsureCreated();

        const string studentId    = "stu-flow-1";
        const string supervisorId = "sup-flow-1";

        ctx.Students.Add(new StudentBuilder()
            .WithId(studentId).WithEmail("student.flow@test.com").Build());
        ctx.Supervisors.Add(new SupervisorBuilder()
            .WithId(supervisorId).WithEmail("sup.flow@test.com")
            .WithMaxProjects(3).WithCurrentProjects(0).Build());
        ctx.Proposals.Add(new ProposalBuilder()
            .WithId(100).WithStudentId(studentId).WithResearchAreaId(1).Build());
        ctx.SupervisorInterests.Add(new SupervisorInterestBuilder()
            .WithId(50).WithSupervisorId(supervisorId).WithProposalId(100).Build());
        await ctx.SaveChangesAsync();

        // ── Act Step 1: GET confirm page to obtain CSRF token ─────────────────
        var client  = _factory.CreateAuthenticatedClient(supervisorId, "Supervisor");
        var getResp = await client.GetAsync("/Interest/ConfirmInterest/50");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK,
            "the confirm-interest page should render for a valid pending interest");

        var html  = await getResp.Content.ReadAsStringAsync();
        var token = BlindMatchWebApplicationFactory.ExtractAntiForgeryToken(html);

        // ── Act Step 2: POST confirm ──────────────────────────────────────────
        var formData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["InterestId"]                 = "50",
            ["__RequestVerificationToken"] = token
        });

        var postResp = await client.PostAsync("/Interest/ConfirmInterest", formData);

        // Expect redirect to MySupervisedMatches (or 200 if AllowAutoRedirect = true)
        postResp.StatusCode.Should().BeOneOf(
            HttpStatusCode.Found,      // 302 redirect
            HttpStatusCode.OK);        // if auto-redirected to matches page

        // ── Assert: verify DB state via a fresh context scope ─────────────────
        using var verifyScope = _factory.Services.CreateScope();
        var db = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Matches.Should().HaveCount(1);
        db.IdentityReveals.Should().HaveCount(1);

        var proposal = db.Proposals.Find(100)!;
        proposal.Status.Should().Be(ProposalStatus.Matched);

        var interest = db.SupervisorInterests.Find(50)!;
        interest.Status.Should().Be(InterestStatus.Confirmed);
        interest.ConfirmedAt.Should().NotBeNull();

        var match = db.Matches.Single();
        match.StudentId.Should().Be(studentId);
        match.SupervisorId.Should().Be(supervisorId);
        match.ProposalId.Should().Be(100);
        match.Status.Should().Be(MatchStatus.Pending);
    }

    // ── TC-FLOW-02: Supervisor browse does not expose student identity ─────────

    [Fact]
    public async Task SupervisorBrowse_ResponseBody_DoesNotContainStudentIdentity()
    {
        // ── Arrange ───────────────────────────────────────────────────────────
        using var seedScope = _factory.Services.CreateScope();
        var ctx = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ctx.Database.EnsureCreated();

        const string studentId    = "stu-anon-1";
        const string supervisorId = "sup-anon-1";
        const string studentEmail = "secret.student@test.com";
        const string studentName  = "Secret Student";

        if (!ctx.Students.Any(s => s.Id == studentId))
        {
            ctx.Students.Add(new StudentBuilder()
                .WithId(studentId).WithEmail(studentEmail).Build());
            ctx.Supervisors.Add(new SupervisorBuilder()
                .WithId(supervisorId).Build());
            ctx.Proposals.Add(new ProposalBuilder()
                .WithId(200).WithStudentId(studentId).WithResearchAreaId(1).Build());
            await ctx.SaveChangesAsync();
        }

        // ── Act ───────────────────────────────────────────────────────────────
        var client = _factory.CreateAuthenticatedClient(supervisorId, "Supervisor");
        var resp   = await client.GetAsync("/SupervisorBrowse/Index");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadAsStringAsync();

        // ── Assert: student identity must not appear ───────────────────────────
        body.Should().NotContain(studentId,   "student internal ID must never appear in browse view");
        body.Should().NotContain(studentEmail,"student email must never appear in browse view");
        body.Should().NotContain(studentName, "student name must never appear in browse view");

        // The anonymised project code SHOULD appear
        body.Should().Contain("Project #", "proposals should show as anonymised project codes");
    }
}
