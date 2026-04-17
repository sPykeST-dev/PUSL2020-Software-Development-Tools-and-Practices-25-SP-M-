using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Infrastructure.Services;
using BlindMatch.Tests.TestHelpers;
using BlindMatch.Tests.TestHelpers.EntityBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Match = BlindMatch.Core.Entities.Match;

namespace BlindMatch.Tests.UnitTests.Services;

public class BlindMatchServiceTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    private static BlindMatchService CreateService(
        ApplicationDbContext ctx,
        Mock<ISupervisorRepository>? supRepo = null,
        Mock<INotificationService>?  notify  = null,
        Mock<IAuditService>?         audit   = null) =>
        new(ctx,
            supRepo?.Object ?? Mock.Of<ISupervisorRepository>(),
            notify?.Object  ?? Mock.Of<INotificationService>(),
            audit?.Object   ?? Mock.Of<IAuditService>(),
            MockHelpers.CreateNullLogger<BlindMatchService>());

    /// <summary>Seeds Student → Proposal → SupervisorInterest in the correct order.</summary>
    private static void SeedValidScenario(
        ApplicationDbContext ctx,
        string studentId     = "stu-1",
        string supervisorId  = "sup-1",
        int    proposalId    = 1,
        int    interestId    = 1,
        ProposalStatus proposalStatus  = ProposalStatus.Submitted,
        InterestStatus interestStatus  = InterestStatus.Pending)
    {
        ctx.Students.Add(new StudentBuilder().WithId(studentId).Build());
        ctx.Proposals.Add(new ProposalBuilder()
            .WithId(proposalId)
            .WithStudentId(studentId)
            .WithStatus(proposalStatus)
            .Build());
        ctx.SupervisorInterests.Add(new SupervisorInterestBuilder()
            .WithId(interestId)
            .WithSupervisorId(supervisorId)
            .WithProposalId(proposalId)
            .WithStatus(interestStatus)
            .Build());
        ctx.SaveChanges();
    }

    private static Mock<ISupervisorRepository> SupervisorWithCapacity(string supervisorId = "sup-1")
    {
        var mock = new Mock<ISupervisorRepository>();
        mock.Setup(r => r.HasCapacityAsync(supervisorId)).ReturnsAsync(true);
        mock.Setup(r => r.IncrementProjectCountAsync(supervisorId)).Returns(Task.CompletedTask);
        return mock;
    }

    // ── Guard: interest not found ─────────────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_InterestNotFound_ReturnsFailure()
    {
        using var ctx = CreateContext();
        var svc = CreateService(ctx);

        var result = await svc.ConfirmInterestAsync(interestId: 99, supervisorId: "sup-1");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Interest record not found.");
    }

    // ── Guard: wrong supervisor ───────────────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_WrongSupervisorId_ReturnsFailure()
    {
        using var ctx = CreateContext();
        SeedValidScenario(ctx, supervisorId: "sup-A");

        var svc    = CreateService(ctx);
        var result = await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-B");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("You are not authorised to confirm this interest.");
    }

    // ── Guard: already confirmed ──────────────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_AlreadyConfirmed_ReturnsFailure()
    {
        using var ctx = CreateContext();
        SeedValidScenario(ctx, interestStatus: InterestStatus.Confirmed);

        var svc    = CreateService(ctx);
        var result = await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This interest has already been confirmed.");
    }

    // ── Guard: proposal already matched ──────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_ProposalAlreadyMatched_ReturnsFailure()
    {
        using var ctx = CreateContext();
        SeedValidScenario(ctx, proposalStatus: ProposalStatus.Matched);

        var svc    = CreateService(ctx);
        var result = await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This proposal has already been matched with another supervisor.");
    }

    // ── Guard: supervisor at capacity ─────────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_NoCapacity_ReturnsFailure()
    {
        using var ctx = CreateContext();
        SeedValidScenario(ctx);

        var supRepo = new Mock<ISupervisorRepository>();
        supRepo.Setup(r => r.HasCapacityAsync("sup-1")).ReturnsAsync(false);

        var svc    = CreateService(ctx, supRepo);
        var result = await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("You have reached your maximum project capacity and cannot take on more projects.");
    }

    // ── Happy path: DB state after confirm ────────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_ValidInput_CreatesMatchAndReveal()
    {
        using var ctx     = CreateContext();
        var supRepo       = SupervisorWithCapacity();
        SeedValidScenario(ctx);

        var svc    = CreateService(ctx, supRepo);
        var result = await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        result.IsSuccess.Should().BeTrue();

        var match = ctx.Matches.Single();
        match.ProposalId.Should().Be(1);
        match.StudentId.Should().Be("stu-1");
        match.SupervisorId.Should().Be("sup-1");
        match.Status.Should().Be(MatchStatus.Pending);

        ctx.IdentityReveals.Should().HaveCount(1);
        ctx.IdentityReveals.Single().TriggeredBySupervisorId.Should().Be("sup-1");

        var proposal = ctx.Proposals.Find(1)!;
        proposal.Status.Should().Be(ProposalStatus.Matched);

        var interest = ctx.SupervisorInterests.Find(1)!;
        interest.Status.Should().Be(InterestStatus.Confirmed);
        interest.ConfirmedAt.Should().NotBeNull();
    }

    // ── Happy path: post-commit notifications ─────────────────────────────────

    [Fact]
    public async Task ConfirmInterest_ValidInput_CallsNotificationAndAudit()
    {
        using var ctx = CreateContext();
        var supRepo   = SupervisorWithCapacity();
        var notify    = new Mock<INotificationService>();
        var audit     = new Mock<IAuditService>();

        notify.Setup(n => n.NotifyMatchCreatedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
              .Returns(Task.CompletedTask);
        audit.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
             .Returns(Task.CompletedTask);

        SeedValidScenario(ctx);

        var svc = CreateService(ctx, supRepo, notify, audit);
        await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        notify.Verify(n => n.NotifyMatchCreatedAsync("stu-1", "sup-1", 1), Times.Once);
        audit.Verify(a => a.LogAsync("ConfirmInterest", "Match", It.IsAny<string?>(), "sup-1", It.IsAny<string?>()), Times.Once);
    }

    // ── Happy path: supervisor project count incremented ─────────────────────

    [Fact]
    public async Task ConfirmInterest_ValidInput_IncrementsProjectCount()
    {
        using var ctx = CreateContext();
        var supRepo   = SupervisorWithCapacity();
        SeedValidScenario(ctx);

        var svc = CreateService(ctx, supRepo);
        await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        supRepo.Verify(r => r.IncrementProjectCountAsync("sup-1"), Times.Once);
    }

    // ── Exception during SaveChanges → failure returned ──────────────────────

    [Fact]
    public async Task ConfirmInterest_DbException_ReturnsFailure()
    {
        // Seed into a normal context first, then open a ThrowingDbContext on
        // the same InMemory database so SaveChangesAsync throws mid-transaction.
        var dbName = Guid.NewGuid().ToString();
        var opts   = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        using var seedCtx = new ApplicationDbContext(opts);
        seedCtx.Database.EnsureCreated();
        seedCtx.Students.Add(new StudentBuilder().WithId("stu-1").Build());
        seedCtx.Proposals.Add(new ProposalBuilder().WithId(1).WithStudentId("stu-1").Build());
        seedCtx.SupervisorInterests.Add(new SupervisorInterestBuilder()
            .WithId(1).WithSupervisorId("sup-1").WithProposalId(1).Build());
        await seedCtx.SaveChangesAsync();

        using var throwingCtx = new ThrowingDbContext(opts);
        var supRepo           = SupervisorWithCapacity();
        var svc               = CreateService(throwingCtx, supRepo);

        var result = await svc.ConfirmInterestAsync(interestId: 1, supervisorId: "sup-1");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("An unexpected error occurred. No changes were saved. Please try again.");
    }

    // ── Inner helper: context that throws on SaveChanges ─────────────────────

    private sealed class ThrowingDbContext(DbContextOptions<ApplicationDbContext> options)
        : ApplicationDbContext(options)
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Simulated database failure.");
    }
}
