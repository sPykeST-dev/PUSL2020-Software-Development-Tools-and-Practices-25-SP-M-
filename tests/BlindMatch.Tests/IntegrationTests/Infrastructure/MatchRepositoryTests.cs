using BlindMatch.Core.Enums;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Infrastructure.Repositories;
using BlindMatch.Tests.IntegrationTests.Infrastructure;
using BlindMatch.Tests.TestHelpers.EntityBuilders;
using Match = BlindMatch.Core.Entities.Match;

namespace BlindMatch.Tests.IntegrationTests.Infrastructure;

public class MatchRepositoryTests
{
    private static ApplicationDbContext CreateContext() =>
        new TestDatabaseFixture().CreateContext();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async Task SeedMatch(
        ApplicationDbContext ctx,
        int id,
        string studentId,
        string supervisorId,
        int proposalId,
        MatchStatus status)
    {
        ctx.Matches.Add(new Match
        {
            Id           = id,
            StudentId    = studentId,
            SupervisorId = supervisorId,
            ProposalId   = proposalId,
            Status       = status,
            CreatedAt    = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();
    }

    private static async Task SeedMatchSet(ApplicationDbContext ctx)
    {
        // Seed Student and Supervisor entities required for navigation properties
        ctx.Students.Add(new StudentBuilder().WithId("stu-m-1").Build());
        ctx.Students.Add(new StudentBuilder().WithId("stu-m-2").Build());
        ctx.Students.Add(new StudentBuilder().WithId("stu-m-3").Build());
        ctx.Students.Add(new StudentBuilder().WithId("stu-m-4").Build());
        ctx.Supervisors.Add(new SupervisorBuilder().WithId("sup-m-1").Build());
        ctx.Proposals.AddRange(
            new ProposalBuilder().WithId(100).WithStudentId("stu-m-1").Build(),
            new ProposalBuilder().WithId(101).WithStudentId("stu-m-2").Build(),
            new ProposalBuilder().WithId(102).WithStudentId("stu-m-3").Build(),
            new ProposalBuilder().WithId(103).WithStudentId("stu-m-4").Build());
        await ctx.SaveChangesAsync();

        await SeedMatch(ctx, 1, "stu-m-1", "sup-m-1", 100, MatchStatus.Pending);
        await SeedMatch(ctx, 2, "stu-m-2", "sup-m-1", 101, MatchStatus.Pending);
        await SeedMatch(ctx, 3, "stu-m-3", "sup-m-1", 102, MatchStatus.Pending);
        await SeedMatch(ctx, 4, "stu-m-4", "sup-m-1", 103, MatchStatus.Approved);
    }

    // ── CountByStatusAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CountByStatus_ReturnsCorrectCountForEachStatus()
    {
        using var ctx = CreateContext();
        await SeedMatchSet(ctx);

        var repo = new MatchRepository(ctx);

        (await repo.CountByStatusAsync(MatchStatus.Pending)).Should().Be(3);
        (await repo.CountByStatusAsync(MatchStatus.Approved)).Should().Be(1);
        (await repo.CountByStatusAsync(MatchStatus.Rejected)).Should().Be(0);
    }

    // ── GetByStatusAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByStatus_ReturnsOnlyMatchesWithRequestedStatus()
    {
        using var ctx = CreateContext();
        await SeedMatchSet(ctx);

        var repo    = new MatchRepository(ctx);
        var pending = await repo.GetByStatusAsync(MatchStatus.Pending);

        pending.Should().HaveCount(3);
        pending.Should().AllSatisfy(m => m.Status.Should().Be(MatchStatus.Pending));
    }

    [Fact]
    public async Task GetByStatus_ReturnsEmptyWhenNoMatchesWithStatus()
    {
        using var ctx = CreateContext();
        await SeedMatchSet(ctx);

        var repo     = new MatchRepository(ctx);
        var rejected = await repo.GetByStatusAsync(MatchStatus.Rejected);

        rejected.Should().BeEmpty();
    }

    // ── ExistsForProposalAsync ────────────────────────────────────────────────

    [Fact]
    public async Task ExistsForProposal_WhenMatchExists_ReturnsTrue()
    {
        using var ctx = CreateContext();
        ctx.Students.Add(new StudentBuilder().WithId("stu-ex-1").Build());
        ctx.Supervisors.Add(new SupervisorBuilder().WithId("sup-ex-1").Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(200).WithStudentId("stu-ex-1").Build());
        await ctx.SaveChangesAsync();
        await SeedMatch(ctx, 50, "stu-ex-1", "sup-ex-1", 200, MatchStatus.Pending);

        var repo   = new MatchRepository(ctx);
        var result = await repo.ExistsForProposalAsync(200);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsForProposal_WhenNoMatch_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo      = new MatchRepository(ctx);

        var result = await repo.ExistsForProposalAsync(999);

        result.Should().BeFalse();
    }
}
