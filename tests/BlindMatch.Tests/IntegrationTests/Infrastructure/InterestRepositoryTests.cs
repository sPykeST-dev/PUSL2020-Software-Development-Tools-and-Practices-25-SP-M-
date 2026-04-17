using BlindMatch.Core.Enums;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Infrastructure.Repositories;
using BlindMatch.Tests.IntegrationTests.Infrastructure;
using BlindMatch.Tests.TestHelpers.EntityBuilders;

namespace BlindMatch.Tests.IntegrationTests.Infrastructure;

public class InterestRepositoryTests
{
    private static ApplicationDbContext CreateContext() =>
        new TestDatabaseFixture().CreateContext();

    private static async Task SeedInterest(
        ApplicationDbContext ctx,
        string supervisorId = "sup-int-1",
        int proposalId      = 1,
        int interestId      = 1)
    {
        ctx.Students.Add(new StudentBuilder().WithId("stu-int-1").Build());
        ctx.Supervisors.Add(new SupervisorBuilder().WithId(supervisorId).Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(proposalId).WithStudentId("stu-int-1").Build());
        ctx.SupervisorInterests.Add(new SupervisorInterestBuilder()
            .WithId(interestId)
            .WithSupervisorId(supervisorId)
            .WithProposalId(proposalId)
            .Build());
        await ctx.SaveChangesAsync();
    }

    // ── ExistsAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Exists_WhenInterestPresent_ReturnsTrue()
    {
        using var ctx = CreateContext();
        await SeedInterest(ctx, supervisorId: "sup-int-1", proposalId: 1);

        var repo   = new InterestRepository(ctx);
        var result = await repo.ExistsAsync("sup-int-1", 1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Exists_WhenNoInterest_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo      = new InterestRepository(ctx);

        var result = await repo.ExistsAsync("sup-int-1", 999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Exists_WrongSupervisor_ReturnsFalse()
    {
        using var ctx = CreateContext();
        await SeedInterest(ctx, supervisorId: "sup-int-1", proposalId: 1);

        var repo   = new InterestRepository(ctx);
        var result = await repo.ExistsAsync("sup-different", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Exists_WrongProposal_ReturnsFalse()
    {
        using var ctx = CreateContext();
        await SeedInterest(ctx, supervisorId: "sup-int-1", proposalId: 1);

        var repo   = new InterestRepository(ctx);
        var result = await repo.ExistsAsync("sup-int-1", 2);

        result.Should().BeFalse();
    }

    // ── GetBySupervisorAndProposalAsync ───────────────────────────────────────

    [Fact]
    public async Task GetBySupervisorAndProposal_WhenExists_ReturnsInterest()
    {
        using var ctx = CreateContext();
        await SeedInterest(ctx, supervisorId: "sup-int-2", proposalId: 5, interestId: 10);

        var repo   = new InterestRepository(ctx);
        var result = await repo.GetBySupervisorAndProposalAsync("sup-int-2", 5);

        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
    }

    [Fact]
    public async Task GetBySupervisorAndProposal_WhenNotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo      = new InterestRepository(ctx);

        var result = await repo.GetBySupervisorAndProposalAsync("sup-nobody", 99);

        result.Should().BeNull();
    }

    // ── GetInterestsBySupervisorAsync ─────────────────────────────────────────

    [Fact]
    public async Task GetInterestsBySupervisor_ReturnsOnlyThatSupervisorsInterests()
    {
        using var ctx = CreateContext();
        ctx.Students.AddRange(
            new StudentBuilder().WithId("stu-gi-1").Build(),
            new StudentBuilder().WithId("stu-gi-2").Build(),
            new StudentBuilder().WithId("stu-gi-3").Build());
        ctx.Supervisors.AddRange(
            new SupervisorBuilder().WithId("sup-gi-1").Build(),
            new SupervisorBuilder().WithId("sup-gi-2").Build());
        ctx.Proposals.AddRange(
            new ProposalBuilder().WithId(60).WithStudentId("stu-gi-1").Build(),
            new ProposalBuilder().WithId(61).WithStudentId("stu-gi-2").Build(),
            new ProposalBuilder().WithId(62).WithStudentId("stu-gi-3").Build());
        ctx.SupervisorInterests.AddRange(
            new SupervisorInterestBuilder().WithId(20).WithSupervisorId("sup-gi-1").WithProposalId(60).Build(),
            new SupervisorInterestBuilder().WithId(21).WithSupervisorId("sup-gi-1").WithProposalId(61).Build(),
            new SupervisorInterestBuilder().WithId(22).WithSupervisorId("sup-gi-2").WithProposalId(62).Build());
        await ctx.SaveChangesAsync();

        var repo   = new InterestRepository(ctx);
        var result = await repo.GetInterestsBySupervisorAsync("sup-gi-1");

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(i => i.SupervisorId.Should().Be("sup-gi-1"));
    }

    // ── ConfirmInterestAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_SetsStatusConfirmedAndTimestamp()
    {
        using var ctx = CreateContext();
        await SeedInterest(ctx, supervisorId: "sup-ci-1", proposalId: 70, interestId: 30);

        var repo   = new InterestRepository(ctx);
        var before = DateTime.UtcNow;
        await repo.ConfirmInterestAsync(30);

        var updated = ctx.SupervisorInterests.Find(30)!;
        updated.Status.Should().Be(InterestStatus.Confirmed);
        updated.ConfirmedAt.Should().NotBeNull();
        updated.ConfirmedAt.Should().BeOnOrAfter(before);
    }
}
